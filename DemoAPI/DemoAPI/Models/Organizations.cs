using DemoAPI.Helpers;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace DemoAPI.Models
{
    public class Organizations
    {
        public int? id { get; set; }
        [JsonProperty("Dr_id")]
        public string? dr_Id { get; set; }
        public string? externalId { get; set; }
        public int? location_Id { get; set; }
        public string? name { get; set; }
        public string? siteType_Code { get; set; }
        public string? status_Code { get; set; }
        public string? doingBusinessAs { get; set; }
        public string? otherName { get; set; }
        public bool displayOtherName { get; set; }
        public bool handicapAccess { get; set; }
        public bool publicTransportation { get; set; }
        public bool hideFromHub { get; set; }
        public int? locationAddresses_Id { get; set; }
        public string? addressLine1 { get; set; }
        public string? addressLine2 { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zip { get; set; }
        public string? ext1 { get; set; }
        public string? phone1 { get; set; }
        public string? phone2 { get; set; }
        public string? ext2 { get; set; }
        public string? fax { get; set; }
        public string? email { get; set; }
        public string? country { get; set; }
        public string? taxID { get; set; }
        public string? groupNPI { get; set; }
        public string? groupPTAN { get; set; }
        public string? medicaidNumber { get; set; }
        public string? medicareNumber { get; set; }
        public string? county { get; set; }

    }
}
