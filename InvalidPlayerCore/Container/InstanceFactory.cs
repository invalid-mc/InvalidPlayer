using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection;

//
//YUKI FOREVER
//
// 
namespace InvalidPlayerCore.Container
{


    public class ActivatorInstanceFactory : IInstanceFactory
    {
        public object GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }



    public class FuncInstanceFactory : IInstanceFactory
    {
        private ConcurrentDictionary<Type, Func<object>> dicEx = new ConcurrentDictionary<Type, Func<object>>();

        public object GetInstance(Type type)
        {
            Func<object> value = null;

            if (dicEx.TryGetValue(type, out value))
            {
                return value();
            }
            else
            {
                value = CreateNewFunc(type);
                dicEx[type] = value;
                return value();
            }
        }


        static Func<object> CreateNewFunc(Type type)
        {
            var newExp = Expression.New(type);
            var lambdaExp = Expression.Lambda<Func<object>>(newExp, null);
            var func = lambdaExp.Compile();
            return func;
        }
    }

}

