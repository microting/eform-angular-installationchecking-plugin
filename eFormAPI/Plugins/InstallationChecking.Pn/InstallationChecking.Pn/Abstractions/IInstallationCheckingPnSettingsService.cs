using System.Threading.Tasks;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.InstallationCheckingBase.Infrastructure.Models;

namespace InstallationChecking.Pn.Abstractions
{
    public interface IInstallationCheckingPnSettingsService
    {
        Task<OperationDataResult<InstallationCheckingBaseSettings>> GetSettings();
        Task<OperationResult> UpdateSettings(InstallationCheckingBaseSettings installationcheckingBaseSettings);
    }
}