// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Collection of adoxio_interest
    /// </summary>
    /// <remarks>
    /// Microsoft.Dynamics.CRM.adoxio_interestCollection
    /// </remarks>
    public partial class MicrosoftDynamicsCRMadoxioInterestCollection
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioInterestCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioInterestCollection()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioInterestCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioInterestCollection(IList<MicrosoftDynamicsCRMadoxioInterest> value = default(IList<MicrosoftDynamicsCRMadoxioInterest>))
        {
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public IList<MicrosoftDynamicsCRMadoxioInterest> Value { get; set; }

    }
}
