namespace Syrx.Commanders.Databases.Tests.Integration
{
    public class AssertionMessages 
    {
        private IDictionary<string, string> _messages;

        public AssertionMessages()
        {
            _messages = new Dictionary<string, string>();
        }

        public void Add<TType>(string method, string message)
        {
            var key = $"{typeof(TType)}.{method}";
            _messages.Add(key, message);
        }

        public string Retrieve<TType>(string method)
        {
            var key = $"{typeof(TType)}.{method}";
            _ = _messages.TryGetValue(key, out var value);

            Throw<ArgumentException>(!string.IsNullOrWhiteSpace(value), $"No assertion message available for '{key}'");

            return value;
        }
    }
    
}
