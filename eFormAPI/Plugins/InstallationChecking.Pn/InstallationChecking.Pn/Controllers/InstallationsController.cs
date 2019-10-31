using System.Threading.Tasks;
using InstallationChecking.Pn.Abstractions;
using InstallationChecking.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.InstallationCheckingBase.Infrastructure.Const;

namespace InstallationChecking.Pn.Controllers
{
    [Authorize(Policy = InstallationCheckingClaims.AccessInstallationCheckingPlugin)]
    public class InstallationsController : Controller
    {
        private readonly IInstallationsService _installationsService;

        public InstallationsController(IInstallationsService installationsService)
        {
            _installationsService = installationsService;
        }

        [HttpGet("api/installationchecking-pn/installations")]
        public async Task<OperationDataResult<InstallationsListModel>> GetInstallationsList(InstallationsRequestModel requestModel)
        {
            return await _installationsService.GetInstallationsList(requestModel);
        }

        [HttpGet("api/installationchecking-pn/installations/{id}")]
        public async Task<OperationDataResult<InstallationModel>> GetInstallation(int id)
        {
            return await _installationsService.GetInstallation(id);
        }

        [HttpPost("api/installationchecking-pn/installations/create")]
        [Authorize(Policy = InstallationCheckingClaims.CreateInstallations)]
        public async Task<OperationResult> CreateInstallation([FromBody] int customerId)
        {
            return await _installationsService.CreateInstallation(customerId);
        }

        [HttpPost("api/installationchecking-pn/installations/assign")]
        [Authorize(Policy = InstallationCheckingClaims.AssignInstallations)]
        public async Task<OperationResult> AssignInstallation([FromBody] InstallationsAssignModel installationsAssignModel)
        {
            return await _installationsService.AssignInstallations(installationsAssignModel);
        }

        [HttpPost("api/installationchecking-pn/installations/retract")]
        [Authorize(Policy = InstallationCheckingClaims.AssignInstallations)]
        public async Task<OperationResult> RetractInstallation([FromBody] int installationId)
        {
            return await _installationsService.RetractInstallation(installationId);
        }

        [HttpPost("api/installationchecking-pn/installations/archive")]
        [Authorize(Policy = InstallationCheckingClaims.ArchiveInstallations)]
        public async Task<OperationResult> ArchiveInstallation([FromBody] int installationId)
        {
            return await _installationsService.ArchiveInstallation(installationId);
        }
    }
}