using System.Threading.Tasks;
using InstallationChecking.Pn.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace InstallationChecking.Pn.Abstractions
{
    public interface IInstallationsService
    {
        Task<OperationDataResult<InstallationModel>> GetInstallation(int id);
        Task<OperationDataResult<InstallationsListModel>> GetInstallationsList(InstallationsRequestModel requestModel);
        Task<OperationResult> CreateInstallation(int customerId);
        Task<OperationResult> AssignInstallations(InstallationsAssignModel installationsAssignModel);
        Task<OperationResult> RetractInstallation(int installationId);
        Task<OperationResult> ArchiveInstallation(int installationId);
    }
}