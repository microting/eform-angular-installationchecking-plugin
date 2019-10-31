using System.Collections.Generic;

namespace InstallationChecking.Pn.Infrastructure.Models
{
    public class InstallationsListModel
    {
        public int Total { get; set; }
        public List<InstallationModel> Installations { get; set; }

        public InstallationsListModel()
        {
            Installations = new List<InstallationModel>();
        }
    }
}