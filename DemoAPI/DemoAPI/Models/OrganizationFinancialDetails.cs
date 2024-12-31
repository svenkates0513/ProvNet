using DemoAPI.Helpers;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace DemoAPI.Models
{
    public class OrganizationFinancialDetails
    {
        public int? Id { get; set; }
        public string? Dr_Id { get; set; }
        public string? Facility_Code { get; set; }
        public int? LocationOtherAddresses_Id { get; set; }
        public bool? UsePhysicalAddress { get; set; }
        public bool? UseMailingAddress { get; set; }
        public bool? IsLockBox { get; set; }
        public string? Cycle_Code { get; set; }
        public bool? BillForServices { get; set; }
        public bool? DeliverBillOffsite { get; set; }
        public bool? IsMedicaidAccepted { get; set; }
        public bool? IsMedicareAccepted { get; set; }
        public bool? ProvideFreeServices { get; set; }
        public string? FinancialInstitution_Code { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public string? RoutingNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? VoidedCheck { get; set; }
        public string? BankLetter { get; set; }
        public int? ClaimsClearinghouse_Id { get; set; }
    }
}
