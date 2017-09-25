using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Strings;
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
            var getImplementedClassTypeMethod = typeof(MockBuilder).GetMethod("GetImplementedClassType", BindingFlags.NonPublic | BindingFlags.Static);

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
            var genericCreateMethod = typeof(MockBuilder).GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Static);

            if (genericCreateMethod == null)
            {
                return null;
            }

            genericCreateMethod = genericCreateMethod.MakeGenericMethod(classType);

            return genericCreateMethod.Invoke(null, new object[] { patternData });
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
        private static object Create<T>(JObject patternData) where T : class, IPublishedContent
        {
            T mock;

            if (typeof(T).IsInterface)
            {
                // no arguments required
                mock = Substitute.For<T>();
            }
            else
            {
                // need to pass in IPublishedContent mock into constructor
                var mockPublishedContent = Substitute.For<IPublishedContent>();

                mock = Substitute.For<T>(mockPublishedContent);
            }

            if (patternData == null)
            {
                // no data JSON found, return empty mock
                return mock;
            }

            // hook up substitute calls
            mock.Name.Returns(GetMockedPublishedProperty("Name", patternData)?.Value);
            mock.GetProperty(Arg.Any<string>(), Arg.Any<bool>()).Returns(callInfo => GetMockedPublishedProperty(callInfo[0] as string, patternData));

            return mock;
        }

        /// <summary>
        /// Creates an instance of MockedPublishedProperty using JSON data from the pattern directory.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="patternData"></param>
        /// <returns></returns>
        private static MockedPublishedProperty GetMockedPublishedProperty(string alias, JObject patternData)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return null;
            }

            // JSON data should use Pascal case names
            alias = alias.ToCleanString(CleanStringType.PascalCase);

            // try to find property value in JSON data
            JToken value;

            if (!patternData.TryGetValue(alias, out value))
            {
                return null;
            }

            object propertyValue = null;

            if (value is JArray)
            {
                var array = (JArray)value;

                // return array items
                propertyValue = array.Values<JObject>().Select(Create<IPublishedContent>).Cast<IPublishedContent>().ToList();
            }
            else
            {
                // return value as string
                propertyValue = value.Value<string>();
            }

            // use normal Umbraco alias name for our mock
            alias = alias.ToCleanString(CleanStringType.Alias);

            return new MockedPublishedProperty(alias, propertyValue, propertyValue, propertyValue);
        }
    }
}
