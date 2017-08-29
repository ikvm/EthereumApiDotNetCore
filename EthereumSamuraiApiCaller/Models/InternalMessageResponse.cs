// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace EthereumSamuraiApiCaller.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class InternalMessageResponse
    {
        /// <summary>
        /// Initializes a new instance of the InternalMessageResponse class.
        /// </summary>
        public InternalMessageResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the InternalMessageResponse class.
        /// </summary>
        public InternalMessageResponse(long? blockNumber = default(long?), int? blockTimeStamp = default(int?), int? depth = default(int?), string fromAddress = default(string), int? messageIndex = default(int?), string toAddress = default(string), string transactionHash = default(string), string type = default(string), string value = default(string))
        {
            BlockNumber = blockNumber;
            BlockTimeStamp = blockTimeStamp;
            Depth = depth;
            FromAddress = fromAddress;
            MessageIndex = messageIndex;
            ToAddress = toAddress;
            TransactionHash = transactionHash;
            Type = type;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "blockNumber")]
        public long? BlockNumber { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "blockTimeStamp")]
        public int? BlockTimeStamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "depth")]
        public int? Depth { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fromAddress")]
        public string FromAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "messageIndex")]
        public int? MessageIndex { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
