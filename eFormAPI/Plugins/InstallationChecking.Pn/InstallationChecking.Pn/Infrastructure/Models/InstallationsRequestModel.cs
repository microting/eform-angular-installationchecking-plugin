using Microting.InstallationCheckingBase.Infrastructure.Enums;

namespace InstallationChecking.Pn.Infrastructure.Models
{
    public class InstallationsRequestModel
    {
        public string SearchString { get; set; }
        public InstallationType? Type { get; set; }
        public InstallationState? State { get; set; }

        public string Sort { get; set; }
        public int PageIndex { get; set; }
        public bool IsSortDsc { get; set; }
        public int PageSize { get; set; }
    }
}