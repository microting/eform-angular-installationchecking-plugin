using System.Collections.Generic;

namespace InstallationChecking.Pn.Infrastructure.Models
{
    public class InstallationsAssignModel
    {
        public int EmployeeId { get; set; }
        public List<int> InstallationIds { get; set; } = new List<int>();
    }
}