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
    /// Collection of adoxio_setting
    /// </summary>
    /// <remarks>
    /// Microsoft.Dynamics.CRM.adoxio_settingCollection
    /// </remarks>
    public partial class MicrosoftDynamicsCRMadoxioSettingCollection
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioSettingCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioSettingCollection()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMadoxioSettingCollection class.
        /// </summary>
        public MicrosoftDynamicsCRMadoxioSettingCollection(IList<MicrosoftDynamicsCRMadoxioSetting> value = default(IList<MicrosoftDynamicsCRMadoxioSetting>))
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
        public IList<MicrosoftDynamicsCRMadoxioSetting> Value { get; set; }

    }
}
