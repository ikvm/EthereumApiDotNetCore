// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.EthereumCoreClient.Models
{
    
    using Lykke.EthereumCoreClient;
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class IssueTokensModel
    {
        /// <summary>
        /// Initializes a new instance of the IssueTokensModel class.
        /// </summary>
        public IssueTokensModel()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the IssueTokensModel class.
        /// </summary>
        public IssueTokensModel(string externalTokenAddress, string toAddress, string amount)
        {
            ExternalTokenAddress = externalTokenAddress;
            ToAddress = toAddress;
            Amount = amount;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "externalTokenAddress")]
        public string ExternalTokenAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "toAddress")]
        public string ToAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (ExternalTokenAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ExternalTokenAddress");
            }
            if (ToAddress == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ToAddress");
            }
            if (Amount == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Amount");
            }
            if (Amount != null)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(Amount, "^[1-9][0-9]*$"))
                {
                    throw new ValidationException(ValidationRules.Pattern, "Amount", "^[1-9][0-9]*$");
                }
            }
        }
    }
}
