using System.Threading.Tasks;
using InstallationChecking.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace InstallationChecking.Pn.Abstractions
{
    public interface IInstallationCheckingPnSettingsService
    {
        Task<OperationDataResult<InstallationCheckingBaseSettings>> GetSettings();
        Task<OperationResult> UpdateSettings(InstallationCheckingBaseSettings installationcheckingBaseSettings);
    }
}