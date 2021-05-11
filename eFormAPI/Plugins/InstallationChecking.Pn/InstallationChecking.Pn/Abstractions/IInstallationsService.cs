using System.Threading.Tasks;
using InstallationChecking.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace InstallationChecking.Pn.Abstractions
{
    using Microting.eFormApi.BasePn.Infrastructure.Models.Common;

    public interface IInstallationsService
    {
        Task<OperationDataResult<Paged<InstallationModel>>> Index(InstallationsRequestModel requestModel);
        Task<OperationDataResult<InstallationModel>> Read(int id);
        Task<OperationResult> Create(int customerId);
        Task<OperationResult> AssignInstallations(InstallationsAssignModel installationsAssignModel);
        Task<OperationResult> RetractInstallation(int installationId);
        Task<OperationResult> ArchiveInstallation(int installationId);
        Task<OperationDataResult<byte[]>> ExportExcel(int installationId);
    }
}