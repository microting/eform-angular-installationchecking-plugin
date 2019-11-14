using Microting.InstallationCheckingBase.Infrastructure.Enums;
using System;

namespace InstallationChecking.Pn.Infrastructure.Models
{
    public class InstallationModel
    {
        public int? Id { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyAddress2 { get; set; }
        public string ZipCode { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }

        public DateTime? DateInstall { get; set; }
        public DateTime? DateRemove { get; set; }
        public DateTime? DateActRemove { get; set; }

        public string AssignedTo { get; set; }

        public InstallationType Type { get; set; }
        public InstallationState State { get; set; }

        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? SdkCaseId { get; set; }
        public int? RemovalFormId { get; set; }
    }
}