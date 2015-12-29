using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InvalidPlayerCore.Exceptions;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public class AutoApplicationContext : IApplicationContext, IBeanFactory
    {
        private static readonly Dictionary<string, BeanDescription> BeanDescriptionCache = new Dictionary<string, BeanDescription>();

        private static readonly ConcurrentDictionary<string, object> BeanCache = new ConcurrentDictionary<string, object>();

        private static readonly Dictionary<Type, HashSet<string>> TypeBeanNameCache = new Dictionary<Type, HashSet<string>>();

        private static readonly Dictionary<string, HashSet<Type>> BeanNameTypeCache = new Dictionary<string, HashSet<Type>>();

        private readonly string _id;

        private readonly IInstanceFactory _instanceFactory;

        public AutoApplicationContext()
        {
            _id = Guid.NewGuid().ToString("N");
            _instanceFactory = new ActivatorInstanceFactory();
        }

        public AutoApplicationContext(string id)
        {
            _id = id;
        }

        public void Scan(List<Assembly> assemblies)
        {
            DoScan(assemblies);
        }

        public void Scan(Assembly assembly)
        {
            DoScan(new List<Assembly>(1) {assembly});
        }


        public void AddBeanDescription(TypeInfo typeInfo, BeanAttribute beanAttribute)
        {
            var name = beanAttribute.Name;
            if (string.IsNullOrEmpty(name))
            {
                name = typeInfo.FullName;
            }

            if (BeanDescriptionCache.ContainsKey(name))
            {
                throw new AppException(string.Format("已经包含name为{0}的Bean!", name));
            }

            var beanDesc = new BeanDescription();
            beanDesc.Name = name;
            beanDesc.BeanType = typeInfo.AsType();
            beanDesc.IsPrototype = beanAttribute is PrototypeAttribute;
            beanDesc.IsSingleton = !beanDesc.IsPrototype;
            BeanDescriptionCache.Add(name, beanDesc);

            var methods = typeInfo.DeclaredMethods;
            foreach (var method in methods)
            {
                var init = method.GetCustomAttribute(typeof (InitAttribute));

                if (init != null)
                {
                    if (method.IsAbstract)
                    {
                        throw new Exception("unsupport abstract init method");
                    }
                    beanDesc.Init = method;
                }
            }

            AddTypeNameCache(beanDesc.BeanType, name);

            //TODO 过滤方式
            var interfaces = typeInfo.ImplementedInterfaces.Where(i => !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Windows"));

            //TODO check if needed
            foreach (var i in interfaces)
            {
                AddTypeNameCache(i, name);
            }
        }


        public void AddTypeNameCache(Type type, string name)
        {
            HashSet<string> names;
            TypeBeanNameCache.TryGetValue(type, out names);
            if (null == names)
            {
                names = new HashSet<string>();
                TypeBeanNameCache.Add(type, names);
            }
            names.Add(name);

            HashSet<Type> types;
            BeanNameTypeCache.TryGetValue(name, out types);
            if (null == types)
            {
                types = new HashSet<Type>();
                BeanNameTypeCache.Add(name, types);
            }
            if (!types.Contains(type))
            {
                types.Add(type);
            }
        }

        public void DoScan(List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var allTypes = assembly.DefinedTypes;

                foreach (var type in allTypes)
                {
                    var beanAttr = type.GetCustomAttribute<BeanAttribute>();
                    if (null != beanAttr)
                    {
                        AddBeanDescription(type, beanAttr);
                    }
                }
            }
        }

        public void Inject(object obj)
        {
            CompleteBean(null, obj.GetType(), obj);
        }


        //TODO 多线程
        public void CompleteBean(BeanDescription desc, Type type, object beanIns)
        {
            var pros = type.GetRuntimeFields();
            foreach (var fieldInfo in pros)
            {
                var injectAttribute = fieldInfo.GetCustomAttribute<InjectAttribute>();
                if (null == injectAttribute)
                {
                    continue;
                }

                var requiredBeanType = fieldInfo.FieldType;
                var requiredBeanTypeInfo = requiredBeanType.GetTypeInfo();
                if (requiredBeanTypeInfo.IsGenericTypeDefinition)
                {
                    throw new ServiceException(string.Format("unsupport type {0}", requiredBeanType.FullName));
                }
                if (requiredBeanTypeInfo.IsGenericType)
                {
                    var genericTypeArguments = requiredBeanTypeInfo.GenericTypeArguments;
                    var listType = typeof (ICollection<>).MakeGenericType(genericTypeArguments);

                    if (listType.GetTypeInfo().IsAssignableFrom(requiredBeanTypeInfo))
                    {
                        if (genericTypeArguments.Length > 1)
                        {
                            throw new Exception("unsupport genericTypeArguments.count >1");
                        }
                        var genericType = genericTypeArguments[0];
                        var names = TypeBeanNameCache[genericType];
                        var constructed = typeof (List<>).MakeGenericType(genericType);
                        dynamic list = Activator.CreateInstance(constructed);
                        foreach (var n in names)
                        {
                            dynamic bean = MakeBean(n);
                            list.Add(bean);
                        }
                        fieldInfo.SetValue(beanIns, list);
                        continue;
                    }
                    throw new ServiceException(string.Format("unsupport type {0}", requiredBeanType.FullName));
                }

                var requiredBeanName = injectAttribute.Name;
                if (string.IsNullOrEmpty(requiredBeanName))
                {
                    var names = TypeBeanNameCache[requiredBeanType];
                    if (names.Count > 1)
                    {
                        throw new ServiceException(string.Format("there are more beans of type {0}", requiredBeanType.FullName));
                    }
                    requiredBeanName = names.First();
                }

                if (!BeanDescriptionCache.ContainsKey(requiredBeanName))
                {
                    throw new ServiceException(string.Format("can find required bean, required name is{0},current bean is {1}", requiredBeanName, type.FullName));
                }

                var requiredBeanIns = MakeBean(requiredBeanName);

                fieldInfo.SetValue(beanIns, requiredBeanIns);
            }

            if (null != desc)
            {
                var init = desc.Init;
                if (null != init)
                {
                    init.Invoke(beanIns, null);
                }
            }
        }

        //TODO 多线程
        public object MakeBean(string name)
        {
            object beanIns = null;
            var beanDesc = BeanDescriptionCache[name];
            var beanType = beanDesc.BeanType;
            if (beanDesc.IsPrototype)
            {
                beanIns = CreateBean(beanType);
                CompleteBean(beanDesc, beanType, beanIns);
            }
            else
            {
                BeanCache.TryGetValue(name, out beanIns);
                if (null == beanIns)
                {
                    beanIns = CreateBean(beanType);
                    beanIns = AddBeanToCache(name, beanIns);
                    CompleteBean(beanDesc, beanType, beanIns);
                }
            }

            if (null == beanIns)
            {
                throw new ServiceException(string.Format("cannot create instance of type {0},name:{1}", beanType, name));
            }

            return beanIns;
        }

        private object CreateBean(Type beanType)
        {
            return _instanceFactory.GetInstance(beanType);
        }

        private object AddBeanToCache(string name, object beanIns)
        {
            if (BeanCache.ContainsKey(name))
            {
                return BeanCache[name];
            }

            BeanCache[name] = beanIns;

            return beanIns;
        }

        /// <summary>
        ///     just new
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNormalBean<T>() where T : new()
        {
            return new T();
        }

        #region  IBeanFactory

        public bool ContainsBean(Type type)
        {
            return TypeBeanNameCache.ContainsKey(type);
        }

        public bool ContainsBean(string name)
        {
            return BeanDescriptionCache.ContainsKey(name);
        }

        public object GetBean(string name)
        {
            return MakeBean(name);
        }

        public T GetBean<T>()
        {
            var type = typeof (T);
            {
                var names = TypeBeanNameCache[type];
                if (names.Count > 1)
                {
                    throw new ServiceException("指定类型含有多种类型Bean定义,请指定name");
                }
                return (T) MakeBean(names.First());
            }
        }

        public T GetBean<T>(string name)
        {
            var types = BeanNameTypeCache[name];
            if (!types.Contains(typeof (T)))
            {
                throw new ServiceException("没有符合指定类型和name的bean");
            }
            return (T) MakeBean(name);
        }


        public Type GetType(string name)
        {
            var desc = BeanDescriptionCache[name];
            return desc.BeanType;
        }

        public bool IsPrototype(string name)
        {
            var desc = BeanDescriptionCache[name];
            return desc.IsPrototype;
        }

        public bool IsSingleton(string name)
        {
            var desc = BeanDescriptionCache[name];
            return desc.IsSingleton;
        }

        #endregion

        #region IApplicationContext

        public Dictionary<string, T> GetBeansOfType<T>()
        {
            var type = typeof (T);
            var names = TypeBeanNameCache[type];
            if (names.Count > 0)
            {
                var result = new Dictionary<string, T>(names.Count);
                foreach (var name in names)
                {
                    var ins = (T) MakeBean(name);
                    result[name] = ins;
                }
                return result;
            }

            return null;
        }

        public string getId()
        {
            return _id;
        }

        #endregion
    }
}