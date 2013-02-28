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
        public delegate T ObjectConstructor<T>(params object[] args);

        /// <summary>
        /// Gets an objects constructor based on the objects namespace + it's name, and the parameters for the constructor.
        /// </summary>
        /// <typeparam name="T">The Type to Return.</typeparam>
        /// <param name="namespaceAndName">The parameters for the constructor. Use a empty array to get the parameterless constructor.</param>
        /// <param name="ctor">The objects Constructor Info.</param>
        /// <returns>The Constructor.</returns>
        public static ObjectConstructor<T> GetObjectConstructor<T>(string namespaceAndName, Type[] parameters)
        {
            Type type = Type.GetType(namespaceAndName);
            if (type == null)
                throw new ArgumentException("The object " + namespaceAndName + " could not be found.");
            var ctorInfo = type.GetConstructor(parameters);
            return GetObjectConstructor<T>(ctorInfo);
        }

        //This Code from: http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
        /// <summary>
        /// Gets an objects constructor based on the ConstructorInfo.
        /// </summary>
        /// <typeparam name="T">The Type to Return.</typeparam>
        /// <param name="ctor">The objects Constructor Info.</param>
        /// <returns>The Constructor.</returns>
        public static ObjectConstructor<T> GetObjectConstructor<T>(ConstructorInfo ctor)
        {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectConstructor<T>), newExp, param);

            //compile it
            ObjectConstructor<T> compiled = (ObjectConstructor<T>)lambda.Compile();
            return compiled;
        }
    }
}
