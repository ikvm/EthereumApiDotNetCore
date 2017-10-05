// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.EthereumCoreClient.Models
{
    
    using Lykke.EthereumCoreClient;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class CheckPendingResponse
    {
        /// <summary>
        /// Initializes a new instance of the CheckPendingResponse class.
        /// </summary>
        public CheckPendingResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CheckPendingResponse class.
        /// </summary>
        public CheckPendingResponse(bool isSynced)
        {
            IsSynced = isSynced;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isSynced")]
        public bool IsSynced { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
