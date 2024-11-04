using DemoAPI.Helpers;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace DemoAPI.Models
{
    public class Demographics
    {
        public int? id { get; set; }

        [JsonProperty("Dr_id")]
        public string? drId { get; set; }
        public string? externalId { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? providerSuffixCode { get; set; }
        public string? providerSalutationCode { get; set; }
        public string? gender { get; set; }
        public DateTime? birthDate { get; set; }
        public DateTime? deathDate { get; set; }
        public string? primaryTitlesCode { get; set; }
        public string? educationDegreeCode { get; set; }
        public int? providerTypesId { get; set; }
        public string? npdbFieldofLicensureCodesCode { get; set; }
        public string? ama { get; set; }
        public string? medicaid { get; set; }
        public string? medicare { get; set; }
        public string? npi { get; set; }
        public string? ssn { get; set; }
        public string? usmle { get; set; }
        public int? caqhid { get; set; }
        public string? caqhUsername { get; set; }
        public string? caqhPassword { get; set; }
        public string? caqhProviderTypeCode { get; set; }
        public string? fninNumber { get; set; }
        public string? fninCountryOfIssueCode { get; set; }
        public string? primaryHomeAddressLine1 { get; set; }
        public string? primaryHomeAddressLine2 { get; set; }
        public string? primaryHomeAddressCity { get; set; }
        public string? primaryHomeAddressState_Code { get; set; }
        public string? primaryHomeAddressZipcode { get; set; }
        public int? primaryHomeAddressCountry_Id { get; set; }
        public string? primaryHomeAddressPhone1 { get; set; }
        public string? primaryHomeAddressPhoneExtension1 { get; set; }
        public string? primaryHomeAddressPhone2 { get; set; }
        public string? primaryHomeAddressPhoneExtension2 { get; set; }
        public string? cellPhone { get; set; }
        public string? fax { get; set; }
        public string? pager { get; set; }
        public string? email { get; set; }
        public string? facebook { get; set; }
        public string? twitter { get; set; }
        public string? homepage { get; set; }
        public string? driverLicenseNumber { get; set; }
        public DateTime? driverLicenseExpirationDate { get; set; }
        public string? driverLicenseStateOfIssue_Code { get; set; }
        public string? ethnicOrigins_Code { get; set; }
        public string? citizenship { get; set; }
        public int? countries_Id { get; set; }
        public string? birthState_Code { get; set; }
        public string? birthCity { get; set; }
        public string? visaTypes_Code { get; set; }
        public string? visaSponsor { get; set; }
        public DateTime? visaIssuedDate { get; set; }
        public DateTime? visaExpiration { get; set; }
        public string? visaNumber { get; set; }
        public string? visaStatus_Code { get; set; }
        public string? primaryPracticeState_Code { get; set; }
        public string? martialStatus_Code { get; set; }
        public string? spouseFullName { get; set; }
        public string? physicianSpouseName { get; set; }
        public string? hStreamID { get; set; }
        public string? document1 { get; set; }
        public string? aliasId { get; set; }
        public bool isOrganization { get; set; }
        public string? appModuleEmail { get; set; }
        public string? appModulePassword { get; set; }
        public string? preferredMethodOfContact_Code { get; set; }
        public bool accessibleFromInternet { get; set; }
        public DateTime? npiEnumerationDate { get; set; }
        public DateTime? caqhReattestationDate { get; set; }
        public DateTime? caqhNextReattestationdate { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(CustomDateConverter))]
        public DateTime? lastModifiedOn { get; set; }
        public string? aoa { get; set; }
        public int? sequence { get; set; }
    }
}
