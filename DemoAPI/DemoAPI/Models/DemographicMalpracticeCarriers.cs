using Newtonsoft.Json;
using System;

namespace DemoAPI.Models
{
    public class DemographicMalpracticeCarriers
    {
        public int? Id { get; set; }
        [JsonProperty("Dr_id")]
        public string? Dr_Id { get; set; }
        public string? Type_Code { get; set; }
        public string? Carrier_Code { get; set; }
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public string? PolicyNumber { get; set; }
        public bool? Tail { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? TicklerDate { get; set; }
        public string? TicklerStatus { get; set; }
        public DateTime? Enrolled { get; set; }
        public string? Document1 { get; set; }
        public bool? PRIVATE { get; set; }
        public string? Facility_Code { get; set; }
        public string? CoverageAggregate { get; set; }
        public string? CoveragePerIncident { get; set; }
        public DateTime? Current_Issue_Date { get; set; }
    }
}
