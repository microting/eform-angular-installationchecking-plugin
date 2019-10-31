using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InstallationChecking.Pn.Abstractions;
using InstallationChecking.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microting.InstallationCheckingBase.Infrastructure.Data;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;
using Microting.eForm.Infrastructure.Constants;

namespace InstallationChecking.Pn.Services
{
    public class InstallationsService : IInstallationsService
    {
        private readonly IInstallationCheckingLocalizationService _installationcheckingLocalizationService;
        private readonly InstallationCheckingPnDbContext _dbContext;
        private readonly IEFormCoreService _coreHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstallationsService(
            InstallationCheckingPnDbContext dbContext,
            IInstallationCheckingLocalizationService installationcheckingLocalizationService,
            IHttpContextAccessor httpContextAccessor,
            IEFormCoreService coreHelper
        )
        {
            _dbContext = dbContext;
            _installationcheckingLocalizationService = installationcheckingLocalizationService;
            _httpContextAccessor = httpContextAccessor;
            _coreHelper = coreHelper;
        }

        public async Task<OperationDataResult<InstallationModel>> GetInstallation(int id)
        {
            try
            {
                var installationModel = await _dbContext.Installations
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.Id == id)
                    .Select(x => new InstallationModel
                    {
                        // TODO
                    }).FirstOrDefaultAsync();

                if (installationModel == null)
                {
                    return new OperationDataResult<InstallationModel>(
                        false,
                        _installationcheckingLocalizationService.GetString("InstallationNotFound"));
                }

                return new OperationDataResult<InstallationModel>(true, installationModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<InstallationModel>(false,
                    _installationcheckingLocalizationService.GetString("ErrorGettingInstallation"));
            }
        }

        public async Task<OperationDataResult<InstallationsListModel>> GetInstallationsList(InstallationsRequestModel requestModel)
        {
            try
            {
                // TODO Sort and filtering
                var list = await _dbContext.Installations
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed)
                    .Select(x => new InstallationModel()
                    {
                        // TODO
                    }
                    ).ToListAsync();

                var listModel = new InstallationsListModel { Total = list.Count(), Installations = list };

                return new OperationDataResult<InstallationsListModel>(true, listModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<InstallationsListModel>(false,
                    _installationcheckingLocalizationService.GetString("ErrorGettingInstallationsList"));
            }
        }

        public async Task<OperationResult> CreateInstallation(int customerId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // TODO

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationcheckingLocalizationService.GetString("InstallationCreatedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false,
                        _installationcheckingLocalizationService.GetString("ErrorWhileCreatingInstallation"));
                }
            }
        }

        public async Task<OperationResult> AssignInstallations(InstallationsAssignModel installationsAssignModel)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // TODO

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationcheckingLocalizationService.GetString("InstallationAssignedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false,
                        _installationcheckingLocalizationService.GetString("ErrorWhileAssigningInstallation"));
                }
            }
        }

        public async Task<OperationResult> RetractInstallation(int installationId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // TODO

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationcheckingLocalizationService.GetString("InstallationRetractedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _installationcheckingLocalizationService.GetString("ErrorWhileRetractingInstallation"));
                }
            }
        }

        public async Task<OperationResult> ArchiveInstallation(int installationId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // TODO

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationcheckingLocalizationService.GetString("InstallationArchivedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _installationcheckingLocalizationService.GetString("ErrorWhileArchivingInstallation"));
                }
            }
        }

        private int UserId
        {
            get
            {
                var value = _httpContextAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }
    }
}