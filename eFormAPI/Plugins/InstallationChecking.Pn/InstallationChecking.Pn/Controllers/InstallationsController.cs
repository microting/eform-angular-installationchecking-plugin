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
    [Route("api/installationchecking-pn/installations")]
    public class InstallationsController : Controller
    {
        private readonly IInstallationsService _installationsService;

        public InstallationsController(IInstallationsService installationsService)
        {
            _installationsService = installationsService;
        }

        [HttpGet()]
        public async Task<OperationDataResult<InstallationsListModel>> Index(InstallationsRequestModel requestModel)
        {
            return await _installationsService.Index(requestModel);
        }
        
        [HttpPost("create")]
        [Authorize(Policy = InstallationCheckingClaims.CreateInstallations)]
        public async Task<OperationResult> Create([FromBody] int customerId)
        {
            return await _installationsService.Create(customerId);
        }

        [HttpGet("{id}")]
        public async Task<OperationDataResult<InstallationModel>> Read(int id)
        {
            return await _installationsService.Read(id);
        }

        [HttpPost("assign")]
        [Authorize(Policy = InstallationCheckingClaims.AssignInstallations)]
        public async Task<OperationResult> AssignInstallation([FromBody] InstallationsAssignModel installationsAssignModel)
        {
            return await _installationsService.AssignInstallations(installationsAssignModel);
        }

        [HttpPost("retract")]
        [Authorize(Policy = InstallationCheckingClaims.AssignInstallations)]
        public async Task<OperationResult> RetractInstallation([FromBody] int installationId)
        {
            return await _installationsService.RetractInstallation(installationId);
        }

        [HttpPost("archive")]
        [Authorize(Policy = InstallationCheckingClaims.ArchiveInstallations)]
        public async Task<OperationResult> ArchiveInstallation([FromBody] int installationId)
        {
            return await _installationsService.ArchiveInstallation(installationId);
        }

        [HttpGet("excel/{id}")]
        public async Task<IActionResult> ExportExcel(int id)
        {
            var result = await _installationsService.ExportExcel(id);

            if (result.Success)
            {
                return File(result.Model, "application/pdf");
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}