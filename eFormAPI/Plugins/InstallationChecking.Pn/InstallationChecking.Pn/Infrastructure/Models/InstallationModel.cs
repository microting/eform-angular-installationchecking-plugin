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

        public int? InstallationEmployeeId { get; set; }
        public int? RemovalEmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? InstallationSdkCaseId { get; set; } // MicrotingUId
        public int? RemovalSdkCaseId { get; set; } // MicrotingUId
        public int? InstallationSdkCaseDbId { get; set; } // Case Id
        public int? RemovalSdkCaseDbId { get; set; } // Case Id
        public int? RemovalFormId { get; set; }
    }
}