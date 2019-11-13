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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection;
using System.IO;
using Microting.eForm.Infrastructure.Models;
using Microting.InstallationCheckingBase.Infrastructure.Models;

namespace InstallationChecking.Pn.Services
{
    public class InstallationsService : IInstallationsService
    {
        private readonly IInstallationCheckingLocalizationService _localizationService;
        private readonly InstallationCheckingPnDbContext _installationCheckingContext;
        private readonly CustomersPnDbAnySql _customersContext;
        private readonly IPluginDbOptions<InstallationCheckingBaseSettings> _options;
        private readonly IEFormCoreService _coreHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InstallationsService(
            InstallationCheckingPnDbContext installationCheckingContext,
            CustomersPnDbAnySql customersContext,
            IPluginDbOptions<InstallationCheckingBaseSettings> options,
            IInstallationCheckingLocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IEFormCoreService coreHelper
        )
        {
            _installationCheckingContext = installationCheckingContext;
            _customersContext = customersContext;
            _options = options;
            _localizationService = localizationService;
            _httpContextAccessor = httpContextAccessor;
            _coreHelper = coreHelper;
        }

        public async Task<OperationDataResult<InstallationModel>> GetInstallation(int id)
        {
            try
            {
                var installationModel = await _installationCheckingContext.Installations.AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.Id == id)
                    .Select(x => new InstallationModel
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
                    return new OperationDataResult<InstallationModel>(false, _localizationService.GetString("InstallationNotFound"));
                }

                return new OperationDataResult<InstallationModel>(true, installationModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<InstallationModel>(false, _localizationService.GetString("ErrorGettingInstallation"));
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
                    listQuery = listQuery.Where(x => x.CompanyName.Contains(requestModel.SearchString));
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
                    listQuery = listQuery.OrderBy(x => x.Id);
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
                return new OperationDataResult<InstallationsListModel>(false, _localizationService.GetString("ErrorGettingInstallationsList"));
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
                        return new OperationDataResult<InstallationModel>(false, _localizationService.GetString("CustomerNotFound"));
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
                    return new OperationResult(true, _localizationService.GetString("InstallationCreatedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false, _localizationService.GetString("ErrorWhileCreatingInstallation"));
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

                    var installationForm = await core.TemplateRead(int.Parse(options.InstallationFormId));
                    var removalForm = await core.TemplateRead(int.Parse(options.RemovalFormId));

                    foreach (var id in installationsAssignModel.InstallationIds)
                    {
                        var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == id);

                        if (installation.State != InstallationState.NotAssigned)
                        {
                            return new OperationResult(false, _localizationService.GetString("InstallationCannotBeAssigned"));
                        }

                        var formId = installation.Type == InstallationType.Installation ? options.InstallationFormId : options.RemovalFormId;
                        var mainElement = await core.TemplateRead(int.Parse(formId));
                        mainElement.Repeated = 0;
                        mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                        mainElement.StartDate = DateTime.Now.ToUniversalTime();

                        installation.EmployeeId = installationsAssignModel.EmployeeId;
                        installation.SdkCaseId = await core.CaseCreate(mainElement, "", installationsAssignModel.EmployeeId);
                        installation.State = InstallationState.Assigned;
                        installation.UpdatedByUserId = UserId;

                        await installation.Update(_installationCheckingContext);
                    }

                    transaction.Commit();
                    return new OperationResult(true, _localizationService.GetString("InstallationAssignedSuccessfully"));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    Trace.TraceError(e.Message);
                    return new OperationResult(false, _localizationService.GetString("ErrorWhileAssigningInstallation"));
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
                        return new OperationResult(false, _localizationService.GetString("InstallationCannotBeRetracted"));
                    }

                    var caseDto = await core.CaseReadByCaseId(installation.SdkCaseId.GetValueOrDefault());

                    if (caseDto != null)
                    {
                        await core.CaseDelete(caseDto.MicrotingUId.GetValueOrDefault());
                    }

                    installation.EmployeeId = null;
                    installation.SdkCaseId = null;
                    installation.State = InstallationState.NotAssigned;
                    installation.UpdatedByUserId = UserId;
                    await installation.Update(_installationCheckingContext);

                    transaction.Commit();
                    return new OperationResult(true, _localizationService.GetString("InstallationRetractedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(false, _localizationService.GetString("ErrorWhileRetractingInstallation"));
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
                        return new OperationResult(false, _localizationService.GetString("InstallationCannotBeArchived"));
                    }

                    installation.State = InstallationState.Archived;
                    installation.UpdatedByUserId = UserId;
                    await installation.Update(_installationCheckingContext);

                    transaction.Commit();
                    return new OperationResult(true, _localizationService.GetString("InstallationArchivedSuccessfully"));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.Message);
                    transaction.Rollback();
                    return new OperationResult(false, _localizationService.GetString("ErrorWhileArchivingInstallation"));
                }
            }
        }

        public async Task<OperationResult> ExportExcel(int installationId)
        {
            try
            {
                var core = await _coreHelper.GetCore();
                var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == installationId);

                if (installation.State != InstallationState.Completed || installation.Type != InstallationType.Removal)
                {
                    return new OperationResult(false, _localizationService.GetString("InstallationCannotBeExported"));
                }

                var caseDto = await core.CaseLookupCaseId(installation.SdkCaseId.GetValueOrDefault());

                if (caseDto == null)
                {
                    return new OperationResult(false, _localizationService.GetString("InstallationCaseNotFound"));
                }

                var reply = await core.CaseRead(caseDto.MicrotingUId.GetValueOrDefault(), caseDto.CheckUId.GetValueOrDefault());
                var checkListValue = (CheckListValue)reply.ElementList[0];
                var fields = checkListValue.DataItemList;

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName().Name;

                using (var templateStream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.template.xlsx"))
                using (var stream = new MemoryStream())
                using (var package = new ExcelPackage(stream, templateStream))
                {
                    var worksheet = package.Workbook.Worksheets[1];

                    foreach (Field field in fields)
                    {
                        // TODO
                    }

                    return new OperationResult(true);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationResult(false, _localizationService.GetString("ErrorWhileExportingExcel"));
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