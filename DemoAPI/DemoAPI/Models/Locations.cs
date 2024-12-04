using Newtonsoft.Json;

namespace DemoAPI.Models
{
    public class Locations
    {
        public int? Id { get; set; }

        [JsonProperty("Dr_id")]
        public string? Dr_Id { get; set; }
        public string? Name { get; set; }
        public string? ExternalId { get; set; }
        public int? LocationAddresses_Id { get; set; }
        public string? SiteType_Code { get; set; }
        public string? Status_Code { get; set; }
        public string? DoingBusinessAs { get; set; }
        public string? OtherName { get; set; }
        public bool? DisplayOtherName { get; set; }
        public string? Region { get; set; }
        public bool? HandicapAccess { get; set; }
        public bool? PublicTransportation { get; set; }
        public bool? HideFromHub { get; set; }
        public string? DisplayName { get; set; }
        public string? Url { get; set; }
    }
}
