using Common.Trackers;

namespace Common.Adapters
{
    public interface IDataAdapter
    {
        void SaveItems(ITracker tracker, IEnumerable<string> shopIds);
    }

    [System.Serializable]
    public class AdapterException : System.Exception
    {
        public string ShopId { get; private set; } = String.Empty;
        public AdapterException() { }
        public AdapterException(string message) : base(message) { }
        public AdapterException(string message, string shopId) : base(message)
        {
            ShopId = shopId;
        }
        public AdapterException(string message, System.Exception inner) : base(message, inner) { }
        protected AdapterException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}