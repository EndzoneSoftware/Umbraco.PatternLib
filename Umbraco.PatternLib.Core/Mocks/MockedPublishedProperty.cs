using Umbraco.Core.Models;

namespace Endzone.Umbraco.PatternLib.Core.Mocks
{
    public class MockedPublishedProperty : IPublishedProperty
    {
        public MockedPublishedProperty(string alias, object dataValue, object value, object xPathValue)
        {
            PropertyTypeAlias = alias;
            DataValue = dataValue;
            Value = value;
            XPathValue = xPathValue;
        }

        public string PropertyTypeAlias { get; }

        public bool HasValue => Value != null;

        public object DataValue { get; }

        public object Value { get; }

        public object XPathValue { get; }
    }
}
