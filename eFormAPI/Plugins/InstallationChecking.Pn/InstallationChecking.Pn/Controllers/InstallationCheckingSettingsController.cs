using System.Threading.Tasks;
using InstallationChecking.Pn.Abstractions;
using InstallationChecking.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace InstallationChecking.Pn.Controllers
{
    [Authorize(Roles = EformRole.Admin)]
    public class InstallationCheckingSettingsController : Controller
    {
        private readonly IInstallationCheckingPnSettingsService _installationCheckingPnSettingsService;

        public InstallationCheckingSettingsController(IInstallationCheckingPnSettingsService installationCheckingPnSettingsService)
        {
            _installationCheckingPnSettingsService = installationCheckingPnSettingsService;
        }

        [HttpGet("api/installationchecking-pn/settings")]
        public async Task<OperationDataResult<InstallationCheckingBaseSettings>> GetSettings()
        {
            return await _installationCheckingPnSettingsService.GetSettings();
        }

        [HttpPost("api/installationchecking-pn/settings")]
        public async Task<OperationResult> UpdateSettings([FromBody] InstallationCheckingBaseSettings installationcheckingBaseSettings)
        {
            return await _installationCheckingPnSettingsService.UpdateSettings(installationcheckingBaseSettings);
        }

        
    }
}