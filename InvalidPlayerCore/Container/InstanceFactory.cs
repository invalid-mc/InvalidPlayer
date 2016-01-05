using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

//
//YUKI FOREVER
//
// 

namespace InvalidPlayerCore.Container
{
    public class ActivatorInstanceFactory : IInstanceFactory
    {
        public object GetInstance(Type beanType)
        {
            var info = beanType.GetTypeInfo();
            return Activator.CreateInstance(beanType);
        }
    }


    public class FuncInstanceFactory : IInstanceFactory
    {
        private readonly ConcurrentDictionary<Type, Func<object>> dicEx = new ConcurrentDictionary<Type, Func<object>>();

        public object GetInstance(Type type)
        {
            Func<object> value = null;

            if (dicEx.TryGetValue(type, out value))
            {
                return value();
            }
            value = CreateNewFunc(type);
            dicEx[type] = value;
            return value();
        }


        private static Func<object> CreateNewFunc(Type type)
        {
            var newExp = Expression.New(type);
            var lambdaExp = Expression.Lambda<Func<object>>(newExp, null);
            var func = lambdaExp.Compile();
            return func;
        }
    }
}