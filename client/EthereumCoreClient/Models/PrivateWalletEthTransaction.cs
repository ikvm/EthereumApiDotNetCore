// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.EthereumCoreClient.Models
{
    using Lykke.EthereumCoreClient;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class PrivateWalletEthTransaction
    {
        /// <summary>
        /// Initializes a new instance of the PrivateWalletEthTransaction
        /// class.
        /// </summary>
        public PrivateWalletEthTransaction()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the PrivateWalletEthTransaction
        /// class.
        /// </summary>
        public PrivateWalletEthTransaction(string fromAddress, string toAddress, string gasAmount, string gasPrice, string value)
        {
            FromAddress = fromAddress;
            ToAddress = toAddress;
            GasAmount = gasAmount;
            GasPrice = gasPrice;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "fromAddress")]
        public string FromAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gasAmount")]
        public string GasAmount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "gasPrice")]
        public string GasPrice { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (FromAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "FromAddress");
            }
            if (ToAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ToAddress");
            }
            if (GasAmount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "GasAmount");
            }
            if (GasPrice == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "GasPrice");
            }
            if (Value == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Value");
            }
            if (GasAmount != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(GasAmount, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "GasAmount", "^[1-9][0-9]*$");
                }
            }
            if (GasPrice != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(GasPrice, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "GasPrice", "^[1-9][0-9]*$");
                }
            }
            if (Value != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Value, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Value", "^[1-9][0-9]*$");
                }
            }
        }
    }
}