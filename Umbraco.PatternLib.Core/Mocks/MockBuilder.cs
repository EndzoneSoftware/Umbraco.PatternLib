using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections;
using System.Linq;
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
            var mock = Substitute.For(new [] { mockType }, null);

            // setup properties using the values supplied in the JSON file
            if (File.Exists(patternDataPath))
            {
                var mockedType = mock.GetType();

                var patternData = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(patternDataPath));

                foreach (var propertyData in patternData.Properties())
                {
                    var property = mockedType.GetProperty(propertyData.Name);

                    if (property == null)
                    {
                        // not found, skip it.
                        continue;
                    }

                    // get value using the expected type
                    object value;

                    if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(String))
                    {
                        // it's a list, get the type of expected item
                        var enumerableType = property.PropertyType.GenericTypeArguments.First();

                        // TODO: not working
                        value = propertyData.Values().Select(x => Substitute.For(new[] { enumerableType }, null)).ToList();
                    }
                    else
                    {
                        // check for standard types
                        switch (property.PropertyType.FullName)
                        {
                            case "System.String":
                                value = propertyData.Value.Value<string>();
                                break;
                            case "System.Int32":
                                value = propertyData.Value.Value<int>();
                                break;
                            case "System.DateTime":
                                value = propertyData.Value.Value<DateTime>();
                                break;
                            default:
                                value = null;
                                break;
                        }
                    }

                    // setup return value for property
                    property.GetValue(mock).Returns(value);
                }
            }

            return mock;
        }

        
    }
}
