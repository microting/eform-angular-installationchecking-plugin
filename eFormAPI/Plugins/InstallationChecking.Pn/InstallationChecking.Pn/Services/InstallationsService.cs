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
using Microting.eFormBaseCustomerBase.Infrastructure.Data;
using Microting.InstallationCheckingBase.Infrastructure.Data.Entities;
using Microting.InstallationCheckingBase.Infrastructure.Enums;
using InstallationChecking.Pn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eForm.Infrastructure.Models;

namespace InstallationChecking.Pn.Services
{
    public class InstallationsService : IInstallationsService
    {
        private readonly IInstallationCheckingLocalizationService _installationCheckingLocalizationService;
        private readonly InstallationCheckingPnDbContext _installationCheckingContext;
        private readonly CustomersPnDbAnySql _customersContext;
        private readonly IPluginDbOptions<InstallationCheckingBaseSettings> _options;
        private readonly IEFormCoreService _coreHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstallationsService(
            InstallationCheckingPnDbContext installationCheckingContext,
            CustomersPnDbAnySql customersContext,
            IPluginDbOptions<InstallationCheckingBaseSettings> options,
            IInstallationCheckingLocalizationService installationcheckingLocalizationService,
            IHttpContextAccessor httpContextAccessor,
            IEFormCoreService coreHelper
        )
        {
            _installationCheckingContext = installationCheckingContext;
            _customersContext = customersContext;
            _options = options;
            _installationCheckingLocalizationService = installationcheckingLocalizationService;
            _httpContextAccessor = httpContextAccessor;
            _coreHelper = coreHelper;
        }

        public async Task<OperationDataResult<InstallationModel>> GetInstallation(int id)
        {
            try
            {
                var installationModel = await _installationCheckingContext.Installations.AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.Id == id)
                    .Select(x =>  new InstallationModel
                        {
                            Id = x.Id,
                            CompanyName = x.CompanyName,
                            CompanyAddress = x.CompanyAddress,
                            CompanyAddress2 = x.CompanyAddress2,
                            CityName = x.CityName,
                            CountryCode = x.CountryCode,
                            ZipCode = x.ZipCode,
                            State = x.State,
                            Type = x.Type,
                            DateInstall = x.DateInstall,
                            DateRemove = x.DateRemove,
                            DateActRemove = x.DateActRemove,
                            EmployeeId = x.EmployeeId,
                            CustomerId = x.CustomerId,
                            SdkCaseId = x.SdkCaseId
                        }
                    ).FirstOrDefaultAsync();

                if (installationModel == null)
                {
                    return new OperationDataResult<InstallationModel>(
                        false,
                        _installationCheckingLocalizationService.GetString("InstallationNotFound"));
                }

                return new OperationDataResult<InstallationModel>(true, installationModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<InstallationModel>(false,
                    _installationCheckingLocalizationService.GetString("ErrorGettingInstallation"));
            }
        }

        public async Task<OperationDataResult<InstallationsListModel>> GetInstallationsList(InstallationsRequestModel requestModel)
        {
            try
            {
                var core = await _coreHelper.GetCore();

                var listQuery = _installationCheckingContext.Installations.AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.State != null)
                {
                    listQuery = listQuery.Where(x => x.State == requestModel.State);
                }

                if (requestModel.Type != null)
                {
                    listQuery = listQuery.Where(x => x.Type == requestModel.Type);
                }

                if (!string.IsNullOrWhiteSpace(requestModel.SearchString))
                {
                    listQuery = listQuery.Where(x => x.CityName.Contains(requestModel.SearchString));
                }

                if (!string.IsNullOrEmpty(requestModel.Sort))
                {
                    if (requestModel.IsSortDsc)
                    {
                        listQuery = listQuery.OrderByDescending(requestModel.Sort);
                    }
                    else
                    {
                        listQuery = listQuery.OrderBy(requestModel.Sort);
                    }
                }
                else
                {
                    listQuery = listQuery
                        .OrderBy(x => x.Id);
                }

                var list = await listQuery
                    .Skip(requestModel.Offset)
                    .Take(requestModel.PageSize)
                    .Select(x => new InstallationModel()
                        {
                            Id = x.Id,
                            CompanyName = x.CompanyName,
                            CompanyAddress = x.CompanyAddress,
                            CompanyAddress2 = x.CompanyAddress2,
                            CityName = x.CityName,
                            CountryCode = x.CountryCode,
                            ZipCode = x.ZipCode,
                            State = x.State,
                            Type = x.Type,
                            DateInstall = x.DateInstall,
                            DateRemove = x.DateRemove,
                            DateActRemove = x.DateActRemove,
                            EmployeeId = x.EmployeeId,
                            CustomerId = x.CustomerId,
                            SdkCaseId = x.SdkCaseId
                        }
                    ).ToListAsync();

                foreach (var item in list.Where(x => x.EmployeeId != null))
                {
                    var site = await core.SiteRead(item.EmployeeId.GetValueOrDefault());
                    item.AssignedTo = site.FirstName + " " + site.LastName;
                }

                var listModel = new InstallationsListModel { Total = list.Count(), Installations = list };

                return new OperationDataResult<InstallationsListModel>(true, listModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<InstallationsListModel>(false,
                    _installationCheckingLocalizationService.GetString("ErrorGettingInstallationsList"));
            }
        }

        public async Task<OperationResult> CreateInstallation(int customerId)
        {
            using (var transaction = await _installationCheckingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = await _customersContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

                    if (customer == null)
                    {
                        return new OperationDataResult<InstallationModel>(
                            false,
                            _installationCheckingLocalizationService.GetString("CustomerNotFound"));
                    }

                    var installation = new Installation()
                    {
                        CompanyName = customer.CompanyName,
                        CompanyAddress = customer.CompanyAddress,
                        CompanyAddress2 = customer.CompanyAddress2,
                        CityName = customer.CityName,
                        CountryCode = customer.CountryCode,
                        ZipCode = customer.ZipCode,
                        State = InstallationState.NotAssigned,
                        Type = InstallationType.Installation,
                        CustomerId = customer.Id,
                        CreatedByUserId = UserId,
                        UpdatedByUserId = UserId
                    };

                    await installation.Create(_installationCheckingContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationCheckingLocalizationService.GetString("InstallationCreatedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false,
                        _installationCheckingLocalizationService.GetString("ErrorWhileCreatingInstallation"));
                }
            }
        }

        public async Task<OperationResult> AssignInstallations(InstallationsAssignModel installationsAssignModel)
        {
            using (var transaction = await _installationCheckingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var core = await _coreHelper.GetCore();
                    var options = _options.Value;

                    foreach (var id in installationsAssignModel.InstallationIds)
                    {
                        MainElement mainElement;
                        var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == id);

                        if (installation.State != InstallationState.Assigned)
                        {
                            return new OperationResult(false,
                                _installationCheckingLocalizationService.GetString("InstallationCannotBeAssigned"));
                        }

                        if (installation.Type == InstallationType.Installation)
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.InstallationFormId));
                        } 
                        else
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.RemovalFormId));
                        }

                        await core.CaseCreate(mainElement, "", installation.EmployeeId.GetValueOrDefault());

                        installation.SdkCaseId = installationsAssignModel.EmployeeId;
                        installation.EmployeeId = installationsAssignModel.EmployeeId;
                        installation.State = InstallationState.Assigned;
                        installation.UpdatedByUserId = UserId;
                        await installation.Update(_installationCheckingContext);
                    }

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationCheckingLocalizationService.GetString("InstallationAssignedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false,
                        _installationCheckingLocalizationService.GetString("ErrorWhileAssigningInstallation"));
                }
            }
        }

        public async Task<OperationResult> RetractInstallation(int installationId)
        {
            using (var transaction = await _installationCheckingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var core = await _coreHelper.GetCore();
                    var options = _options.Value;

                    var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == installationId);

                    if (installation.State != InstallationState.Assigned)
                    {
                        return new OperationResult(false,
                            _installationCheckingLocalizationService.GetString("InstallationCannotBeRetracted"));
                    }

                    if (installation.Type == InstallationType.Installation)
                    {
                        await core.CaseDelete(int.Parse(options.InstallationFormId), installation.EmployeeId.GetValueOrDefault());
                    }
                    else
                    {
                        await core.CaseDelete(int.Parse(options.RemovalFormId), installation.EmployeeId.GetValueOrDefault());
                    }

                    installation.SdkCaseId = null;
                    installation.EmployeeId = null;
                    installation.State = InstallationState.NotAssigned;
                    installation.UpdatedByUserId = UserId;
                    await installation.Update(_installationCheckingContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationCheckingLocalizationService.GetString("InstallationRetractedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _installationCheckingLocalizationService.GetString("ErrorWhileRetractingInstallation"));
                }
            }
        }

        public async Task<OperationResult> ArchiveInstallation(int installationId)
        {
            using (var transaction = await _installationCheckingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == installationId);

                    if (installation.State != InstallationState.Completed || installation.Type != InstallationType.Removal)
                    {
                        return new OperationResult(false,
                            _installationCheckingLocalizationService.GetString("InstallationCannotBeArchived"));
                    }

                    installation.State = InstallationState.Archived;
                    installation.UpdatedByUserId = UserId;
                    await installation.Update(_installationCheckingContext);

                    transaction.Commit();
                    return new OperationResult(
                        true,
                        _installationCheckingLocalizationService.GetString("InstallationArchivedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(
                        false,
                        _installationCheckingLocalizationService.GetString("ErrorWhileArchivingInstallation"));
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