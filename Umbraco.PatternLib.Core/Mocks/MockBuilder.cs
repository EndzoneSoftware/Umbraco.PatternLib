using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using File = System.IO.File;

namespace Endzone.Umbraco.PatternLib.Core.Mocks
{
    internal class MockBuilder
    {
        /// <summary>
        /// Creates a mock of the supplied model type and populates it with any data from the pattern directory.
        /// </summary>
        /// <param name="mockType"></param>
        /// <param name="patternDataPath"></param>
        /// <returns></returns>
        public static object Create(Type mockType, string patternDataPath)
        {
            if (!typeof(IPublishedContent).IsAssignableFrom(mockType))
            {
                // doesn't inherit from IPublishedContent
                return null;
            }

            // get implemented class of the passed in interface
            var getImplementedClassTypeMethod = typeof(MockBuilder).GetMethod("GetImplementedClassType",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (getImplementedClassTypeMethod == null)
            {
                return null;
            }

            getImplementedClassTypeMethod = getImplementedClassTypeMethod.MakeGenericMethod(mockType);

            var classType = getImplementedClassTypeMethod.Invoke(null, null) as Type;

            if (classType == null)
            {
                return null;
            }

            // get pattern data from directory
            JObject patternData = null;

            if (File.Exists(patternDataPath))
            {
                patternData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(patternDataPath));
            }

            // create a mock of the class type
            var genericCreateMethod =
                typeof(MockBuilder).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);

            if (genericCreateMethod == null)
            {
                return null;
            }

            genericCreateMethod = genericCreateMethod.MakeGenericMethod(classType);

            return genericCreateMethod.Invoke(null, new object[] {patternData});
        }

        /// <summary>
        /// Finds the first class implementation of the provided interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static Type GetImplementedClassType<T>()
        {
            var interfaceType = typeof(T);

            if (!interfaceType.IsInterface)
            {
                // it's already a class type
                return interfaceType;
            }

            // based on convention the class name we're looking for is the same as the interface name without the 'I'.
            // i.e. IClassName => ClassName
            var className = interfaceType.Name.TrimStart('I');

            return TypeFinder.FindClassesOfType<T>().FirstOrDefault(x => x.Name == className);
        }

        /// <summary>
        /// Creates a mock of the supplied model type and populates it with any data from the pattern directory.
        /// </summary>
        /// <param name="patternData"></param>
        /// <returns></returns>
        private static T Create<T>(JObject patternData) where T : class, IPublishedContent
        {
            // get default constructor
            var constructor = typeof(T).GetConstructor(new[] { typeof(IPublishedContent) });

            if (constructor == null)
            {
                return default(T);
            }

            // need to pass in IPublishedContent mock into constructor
            var mockPublishedContent = new MockedPublishedContent(patternData);

            var mock = constructor.Invoke(new object[] { mockPublishedContent }) as T;

            return mock;
        }
    }
}
