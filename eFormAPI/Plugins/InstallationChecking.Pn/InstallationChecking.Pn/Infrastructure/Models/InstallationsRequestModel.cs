using Microting.InstallationCheckingBase.Infrastructure.Enums;

namespace InstallationChecking.Pn.Infrastructure.Models
{
    using Microting.eFormApi.BasePn.Infrastructure.Models.Common;

    public class InstallationsRequestModel : PaginationModel
    {
        public string NameFilter { get; set; }

        public InstallationType? Type { get; set; }
            = InstallationType.Installation;

        public InstallationState? State { get; set; }

        public string Sort { get; set; }

        public bool IsSortDsc { get; set; }
    }
}