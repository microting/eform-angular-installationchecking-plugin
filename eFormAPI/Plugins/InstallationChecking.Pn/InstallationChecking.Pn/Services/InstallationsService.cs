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
using System.Reflection;
using System.IO;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
                            SdkCaseId = x.SdkCaseId,
                            RemovalFormId = x.RemovalFormId
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
                            SdkCaseId = x.SdkCaseId,
                            RemovalFormId = x.RemovalFormId
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

                    foreach (var id in installationsAssignModel.InstallationIds)
                    {
                        var installation = await _installationCheckingContext.Installations
                            .Include(i => i.Meters).FirstOrDefaultAsync(x => x.Id == id);

                        if (installation.State != InstallationState.NotAssigned)
                        {
                            return new OperationResult(false, _localizationService.GetString("InstallationCannotBeAssigned"));
                        }
                        System.Diagnostics.Debugger.Break();

                        MainElement mainElement;

                        if (installation.Type == InstallationType.Installation)
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.InstallationFormId));
                        }
                        else
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.RemovalFormId));

                            var dataElement = (DataElement) mainElement.ElementList[0];
                            var removalDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                            dataElement.Description.InderValue = 
                                $"{installation.CompanyAddress}<br>{installation.CompanyAddress2}<br>{installation.ZipCode}<br>{installation.CityName}<br>{installation.CountryCode}<br><b>Nedtagningsdato: {removalDate}</b>";

                            var entityGroup = await core.EntityGroupCreate(
                                Constants.FieldTypes.EntitySearch,
                                "Removal devices " + installation.Id
                            );

                            var i = 0;
                            foreach (var meter in installation.Meters)
                            {
                                await core.EntitySearchItemCreate(
                                    entityGroup.Id,
                                    meter.QR,
                                    "",
                                    i++.ToString()
                                );

                                dataElement.DataItemList.Add(new EntitySearch(
                                    3 + i,
                                    false,
                                    false,
                                    $"Måler {i} - QR",
                                    "",
                                    "e8eaf6",
                                    i,
                                    false,
                                    0,
                                    int.Parse(entityGroup.MicrotingUUID),
                                    false,
                                    "",
                                    3,
                                    false,
                                    "")
                                );
                            }
                            
                            installation.RemovalFormId = int.Parse(options.RemovalFormId);
                        }
                        
                        mainElement.Repeated = 0;
                        mainElement.EndDate = DateTime.UtcNow.AddYears(10);
                        mainElement.StartDate = DateTime.UtcNow;

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

                    var installation = await _installationCheckingContext.Installations.FirstOrDefaultAsync(x => x.Id == installationId);

                    if (installation.State != InstallationState.Assigned)
                    {
                        return new OperationResult(false, _localizationService.GetString("InstallationCannotBeRetracted"));
                    }

                    await core.CaseDelete(installation.SdkCaseId.GetValueOrDefault());

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

        public async Task<OperationDataResult<byte[]>> ExportExcel(int installationId)
        {
            try
            {
                var core = await _coreHelper.GetCore();
                var installation = await _installationCheckingContext.Installations
                    .Include(i => i.Meters)
                    .FirstOrDefaultAsync(x => x.Id == installationId);

                if (installation.State != InstallationState.Completed || installation.Type != InstallationType.Removal)
                {
                    return new OperationDataResult<byte[]>(false, _localizationService.GetString("InstallationCannotBeExported"));
                }

                var caseDto = await core.CaseLookupMUId(installation.SdkCaseId.GetValueOrDefault());

                if (caseDto == null)
                {
                    return new OperationDataResult<byte[]>(false, _localizationService.GetString("InstallationCaseNotFound"));
                }

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName().Name;

                using (var templateStream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.template.xlsx"))
                using (var stream = new MemoryStream())
                using (var package = new ExcelPackage(stream, templateStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var row = 12;

                    foreach (var meter in installation.Meters)
                    {
                        worksheet.Cells[row, 1].Value = installation.CadastralNumber;
                        worksheet.Cells[row, 2].Value = installation.PropertyNumber;
                        worksheet.Cells[row, 3].Value = installation.CompanyName;
                        worksheet.Cells[row, 4].Value = installation.ApartmentNumber;
                        worksheet.Cells[row, 5].Value = installation.CompanyAddress;
                        worksheet.Cells[row, 6].Value = installation.CompanyAddress2;
                        worksheet.Cells[row, 7].Value = installation.ZipCode;
                        worksheet.Cells[row, 8].Value = installation.CityName;
                        worksheet.Cells[row, 9].Value = installation.CountryCode;
                        worksheet.Cells[row, 10].Value = installation.CadastralType;
                        worksheet.Cells[row, 11].Value = ""; // Foundation type
                        worksheet.Cells[row, 12].Value = ""; // Ventilation
                        worksheet.Cells[row, 13].Value = ""; // Household water
                        worksheet.Cells[row, 14].Value = installation.YearBuilt;
                        worksheet.Cells[row, 15].Value = ""; // Renovated
                        worksheet.Cells[row, 16].Value = ""; // Swedish "blue concrete"
                        worksheet.Cells[row, 17].Value = ""; // Visit by a professional
                        worksheet.Cells[row, 18].Value = installation.LivingFloorsNumber;
                        worksheet.Cells[row, 19].Value = meter.QR;
                        worksheet.Cells[row, 20].Value = meter.RoomType; 
                        worksheet.Cells[row, 21].Value = meter.Floor;
                        worksheet.Cells[row, 22].Value = meter.RoomName;
                        worksheet.Cells[row, 23].Value = installation.DateInstall;
                        worksheet.Cells[row, 24].Value = installation.DateActRemove;

                        var site = await core.SiteRead(installation.EmployeeId.GetValueOrDefault());
                        worksheet.Cells[row, 25].Value = site.FirstName + " " + site.LastName;

                        row++;
                    }
                    
                    package.Save();
                    var bytes = package.GetAsByteArray();

                    return new OperationDataResult<byte[]>(true, bytes);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<byte[]>(false, _localizationService.GetString("ErrorWhileExportingExcel"));
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