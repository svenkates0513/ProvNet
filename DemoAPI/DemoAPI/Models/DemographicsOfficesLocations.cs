using Newtonsoft.Json;

namespace DemoAPI.Models
{
    public class DemographicsOfficesLocations
    {
        public int? Id { get; set; }

        [JsonProperty("Dr_id")]
        public string? Dr_Id { get; set; }
        public int? Location_Id { get; set; }
        public string? Status_Code { get; set; }
        public string? Facility_Code { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool? AcceptsNewPatients { get; set; }
        public bool? HideFromHub { get; set; }
        public bool? IncludeInDirectory { get; set; }
        public string? DirectMessagingAddress { get; set; }
        public bool? Telehealth { get; set; }
        public string? PCPSpecialty { get; set; }
        public string? Phone { get; set; }
        public string? Extension { get; set; }
        public string? Phone2 { get; set; }
        public string? Extension2 { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public int? Sequence { get; set; }
        public string? Comment { get; set; }
    }
}
