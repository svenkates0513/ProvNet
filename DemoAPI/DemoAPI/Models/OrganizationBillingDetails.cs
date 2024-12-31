using DemoAPI.Helpers;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace DemoAPI.Models
{
    public class OrganizationBillingDetails
    {
        public int? Id { get; set; }
        [JsonProperty("Dr_id")]
        public string? Dr_Id { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public string? RoutingNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? VoidedCheck { get; set; }
        public string? BankLetter { get; set; }
    }
}
