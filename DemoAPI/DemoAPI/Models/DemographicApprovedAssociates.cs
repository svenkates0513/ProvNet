using Newtonsoft.Json;
using System;

namespace DemoAPI.Models
{
    public class DemographicApprovedAssociates
    {
        public int? Id { get; set; }
        [JsonProperty("Dr_id")]
        public string? Dr_Id { get; set; }
        public string? Type_Code { get; set; }
        public string? Facility_Code { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? InternalID { get; set; }
        public DateTime? From { get; set; }
        public DateTimeKind? To { get; set; }
        public string? ProcedureAuthorized { get; set; }
        public string? Comment { get; set; }
        public bool? Active { get; set; }
        public bool? HideFromHub { get; set; }
        public string? HideFromHubReason { get; set; }
        public string? Document1 { get; set; }
        public bool? Primary { get; set; }
        public bool? Alternate { get; set; }
    }
}
