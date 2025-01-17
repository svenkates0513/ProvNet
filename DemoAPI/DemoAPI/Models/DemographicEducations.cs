using Newtonsoft.Json;
using System;

namespace DemoAPI.Models
{
    public class DemographicEducations
    {
        public int? Id { get; set; }
        public string? Dr_Id { get; set; }
        public string? Type_Code { get; set; }
        public string? Institution_Code { get; set; }
        public string? Facility_Code { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Degree_Code { get; set; }
        public string? Major { get; set; }
        public string? Honors { get; set; }
        public string? ProgramType { get; set; }
        public int? Speciality_Id { get; set; }
        public bool? ProgramCompleted { get; set; }
        public DateTime? AnticipatedCompletionDate { get; set; }
        public bool? ForeignGraduate { get; set; }
        public bool? ECFMGCertified { get; set; }
        public string? ECFMGNumber { get; set; }
        public DateTime? ECFMGIssueDate { get; set; }
        public string? DirectorFirstName { get; set; }
        public string? DirectorLastName { get; set; }
        public string? DirectorTitle { get; set; }
        public string? DirectorSuffix { get; set; }
        public string? Comments { get; set; }
        public string? Documents1 { get; set; }
        public int? Sequence { get; set; }
    }
}
