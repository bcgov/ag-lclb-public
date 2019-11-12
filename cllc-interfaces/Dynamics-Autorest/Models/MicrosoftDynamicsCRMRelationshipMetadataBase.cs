// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Gov.Lclb.Cllb.Interfaces.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Microsoft.Dynamics.CRM.RelationshipMetadataBase
    /// </summary>
    public partial class MicrosoftDynamicsCRMRelationshipMetadataBase
    {
        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMRelationshipMetadataBase class.
        /// </summary>
        public MicrosoftDynamicsCRMRelationshipMetadataBase()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// MicrosoftDynamicsCRMRelationshipMetadataBase class.
        /// </summary>
        public MicrosoftDynamicsCRMRelationshipMetadataBase(bool? isCustomRelationship = default(bool?), string isCustomizable = default(string), bool? isValidForAdvancedFind = default(bool?), string schemaName = default(string), string securityTypes = default(string), bool? isManaged = default(bool?), string relationshipType = default(string), string introducedVersion = default(string))
        {
            IsCustomRelationship = isCustomRelationship;
            IsCustomizable = isCustomizable;
            IsValidForAdvancedFind = isValidForAdvancedFind;
            SchemaName = schemaName;
            SecurityTypes = securityTypes;
            IsManaged = isManaged;
            RelationshipType = relationshipType;
            IntroducedVersion = introducedVersion;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsCustomRelationship")]
        public bool? IsCustomRelationship { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsCustomizable")]
        public string IsCustomizable { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsValidForAdvancedFind")]
        public bool? IsValidForAdvancedFind { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SchemaName")]
        public string SchemaName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "SecurityTypes")]
        public string SecurityTypes { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsManaged")]
        public bool? IsManaged { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "RelationshipType")]
        public string RelationshipType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IntroducedVersion")]
        public string IntroducedVersion { get; set; }

    }
}
