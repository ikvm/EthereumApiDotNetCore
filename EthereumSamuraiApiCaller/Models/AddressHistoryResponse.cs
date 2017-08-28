// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace EthereumSamuraiApiCaller.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class AddressHistoryResponse
    {
        /// <summary>
        /// Initializes a new instance of the AddressHistoryResponse class.
        /// </summary>
        public AddressHistoryResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AddressHistoryResponse class.
        /// </summary>
        public AddressHistoryResponse(long? blockNumber = default(long?), int? blockTimestamp = default(int?), string fromProperty = default(string), bool? hasError = default(bool?), int? messageIndex = default(int?), string to = default(string), string transactionHash = default(string), int? transactionIndex = default(int?), string value = default(string))
        {
            BlockNumber = blockNumber;
            BlockTimestamp = blockTimestamp;
            FromProperty = fromProperty;
            HasError = hasError;
            MessageIndex = messageIndex;
            To = to;
            TransactionHash = transactionHash;
            TransactionIndex = transactionIndex;
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
        [JsonProperty(PropertyName = "blockTimestamp")]
        public int? BlockTimestamp { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public string FromProperty { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hasError")]
        public bool? HasError { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "messageIndex")]
        public int? MessageIndex { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transactionHash")]
        public string TransactionHash { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transactionIndex")]
        public int? TransactionIndex { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }
}
