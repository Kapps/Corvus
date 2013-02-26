using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Corvus.Components.Gameplay
{
    /// <summary>
    /// A static class for generic helper functions.
    /// </summary>
    public static class Helper
    {
        private delegate T ObjectActivator<T>(params object[] args);

        //Modified version of: http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
        /// <summary>
        /// Dynamically instantiates an object.
        /// </summary>
        /// <typeparam name="T">Value type to return.</typeparam>
        /// <param name="type">The Type to instantiate</param>
        public static T GetObject<T>(Type type)
        {
            ConstructorInfo ctor = type.GetConstructor(new Type[] { });
            //make a NewExpression that calls the constructor of this object.
            NewExpression newExp = Expression.New(ctor);

            //create a lambda with the NewExpression as body 
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, Expression.Parameter(typeof(object[]), "args"));

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            T result = compiled();
            return result;
        }

    }
}
