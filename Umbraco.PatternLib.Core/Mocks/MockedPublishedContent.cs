using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;

namespace Endzone.Umbraco.PatternLib.Core.Mocks
{
    public class MockedPublishedContent : IPublishedContent
    {
        /// <summary>
        /// Creates instance of MockedPublishedContent using JSON data for its properties.
        /// </summary>
        /// <param name="mockData"></param>
        public MockedPublishedContent(JObject mockData)
        {
            Properties = mockData.Properties().Select(x => GetMockedPublishedProperty(x.Name, mockData)).ToArray();

            // check if item is content or media based on type alias in properties
            var hasMediaTypeAlias = Properties.Any(x =>
                x.PropertyTypeAlias.Equals("mediaTypeAlias", StringComparison.InvariantCultureIgnoreCase));

            ItemType = (hasMediaTypeAlias ? PublishedItemType.Media : PublishedItemType.Content);
        }

        /// <summary>
        /// Creates an instance of MockedPublishedProperty using property value data from JSON.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="patternData"></param>
        /// <returns></returns>
        private MockedPublishedProperty GetMockedPublishedProperty(string alias, JObject patternData)
        {
            var propertyValue = GetPropertyValue<object>(alias, patternData);

            return new MockedPublishedProperty(alias, propertyValue, propertyValue, propertyValue);
        }

        /// <summary>
        /// Gets property value in specified type, using JSON data.
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="patternData"></param>
        /// <returns></returns>
        private T GetPropertyValue<T>(string alias, JObject patternData)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return default(T);
            }

            // try to find property value in JSON data
            if (!patternData.TryGetValue(alias, StringComparison.InvariantCultureIgnoreCase, out var value))
            {
                return default(T);
            }

            object propertyValue = null;

            if (value is JArray array)
            {
                // return array items
                propertyValue = array.Values<JObject>().Select(x => new MockedPublishedContent(x).ToTypedModel()).ToList();
            }
            else if (value is JValue)
            {
                var stringValue = value.Value<string>();

                // check if it's a specific type
                if (DateTime.TryParse(stringValue, out var date))
                {
                    // datetime
                    propertyValue = date;
                }
                else if (int.TryParse(stringValue, out var integer))
                {
                    // integer
                    propertyValue = integer;
                }
                else if (stringValue.StartsWith("<p") || stringValue.StartsWith("<div") ||
                         stringValue.StartsWith("<span"))
                {
                    // HTML (from RTE)
                    propertyValue = new HtmlString(stringValue);
                }
                else
                {
                    // return value as string
                    propertyValue = stringValue;
                }
            }
            else
            {
                // value is an object, assume it's IPublishedContent
                var obj = value.Value<JObject>();

                propertyValue = new MockedPublishedContent(obj).ToTypedModel();
            }

            if (typeof(T) == typeof(object))
            {
                return (T)((object)propertyValue);
            }
            else
            {
                return (T)Convert.ChangeType(propertyValue, typeof(T));
            }
        }

        /// <summary>
        /// Wraps the current instance in the typed model class.
        /// </summary>
        /// <returns></returns>
        private IPublishedContent ToTypedModel()
        {
            return !PublishedContentModelFactoryResolver.HasCurrent ? 
                this : PublishedContentModelFactoryResolver.Current.Factory.CreateModel(this);
        }

        // custom properties
        public object this[string alias] => GetProperty(alias);

        public IPublishedProperty GetProperty(string alias) => GetProperty(alias, false);

        public IPublishedProperty GetProperty(string alias, bool recurse) =>
            Properties.FirstOrDefault(x => x.PropertyTypeAlias.Equals(alias, StringComparison.InvariantCultureIgnoreCase));

        public ICollection<IPublishedProperty> Properties { get; }

        public PublishedItemType ItemType { get; }

        // standard IPublishedContent fields
        public string DocumentTypeAlias => GetProperty(ItemType == PublishedItemType.Content ? "documentTypeAlias" : "mediaTypeAlias")?.Value as string ?? string.Empty;
        public PublishedContentType ContentType => PublishedContentType.Get(ItemType, DocumentTypeAlias);
        public string Name => GetProperty("name")?.Value as string ?? string.Empty;
        public DateTime CreateDate => GetProperty("createDate")?.Value as DateTime? ?? DateTime.Now;
        public DateTime UpdateDate => GetProperty("updateDate")?.Value as DateTime? ?? DateTime.Now;

        // these can be ignored for now
        public int GetIndex() => -1;
        public IEnumerable<IPublishedContent> ContentSet => Enumerable.Empty<IPublishedContent>();
        public int Id => -1;
        public int TemplateId => -1;
        public int SortOrder => -1;
        public string UrlName => string.Empty;
        public int DocumentTypeId => -1;
        public string WriterName => string.Empty;
        public string CreatorName => string.Empty;
        public int WriterId => 0;
        public int CreatorId => 0;
        public string Path => string.Empty;
        public Guid Version => Guid.NewGuid();
        public int Level => -1;
        public string Url => string.Empty;
        public bool IsDraft => false;
        public IPublishedContent Parent => null;
        public IEnumerable<IPublishedContent> Children => Enumerable.Empty<IPublishedContent>();
    }
}
