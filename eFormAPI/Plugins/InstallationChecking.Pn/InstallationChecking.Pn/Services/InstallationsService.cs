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
using Microting.eFormBaseCustomerBase.Infrastructure.Data;
using Microting.InstallationCheckingBase.Infrastructure.Data.Entities;
using Microting.InstallationCheckingBase.Infrastructure.Enums;
using InstallationChecking.Pn.Infrastructure.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using OfficeOpenXml;
using System.Reflection;
using System.IO;
using Castle.Core.Internal;
using ImageMagick;
using Microting.eForm.Dto;
using Microting.eForm.Infrastructure.Models;
using Microting.eFormApi.BasePn.Infrastructure.Helpers;
using Microting.InstallationCheckingBase.Infrastructure.Models;
using OpenStack.NetCoreSwiftClient.Extensions;
using Constants = Microting.eForm.Infrastructure.Constants.Constants;

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
                            InstallationEmployeeId = x.InstallationEmployeeId,
                            RemovalEmployeeId = x.RemovalEmployeeId,
                            CustomerId = x.CustomerId,
                            InstallationSdkCaseId = x.InstallationSdkCaseId,
                            RemovalSdkCaseId = x.RemovalSdkCaseId,
                            RemovalFormId = x.RemovalFormId,
                        }
                    ).FirstOrDefaultAsync();

                if (installationModel == null)
                {
                    return new OperationDataResult<InstallationModel>(false, _localizationService.GetString("InstallationNotFound"));
                }

                if (installationModel.InstallationEmployeeId != null && installationModel.Type == InstallationType.Installation)
                {
                    var core = await _coreHelper.GetCore();
                    var site = await core.SiteRead(installationModel.InstallationEmployeeId.GetValueOrDefault());
                    installationModel.AssignedTo = site.FirstName + " " + site.LastName;
                    if (installationModel.InstallationSdkCaseId != null)
                    {
                        var sdkCaseId = (int)installationModel.InstallationSdkCaseId;
                        var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                        if (caseLookup?.CheckUId != null)
                        {
                            installationModel.InstallationSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                        }
                    }
                }
                
                if (installationModel.RemovalEmployeeId != null && installationModel.Type == InstallationType.Removal)
                {
                    var core = await _coreHelper.GetCore();
                    var site = await core.SiteRead(installationModel.RemovalEmployeeId.GetValueOrDefault());
                    installationModel.AssignedTo = site.FirstName + " " + site.LastName;
                    if (installationModel.RemovalSdkCaseId != null)
                    {
                        var sdkCaseId = (int)installationModel.RemovalSdkCaseId;
                        var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                        if (caseLookup?.CheckUId != null)
                        {
                            installationModel.RemovalSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                        }
                    }
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
                            InstallationEmployeeId = x.InstallationEmployeeId,
                            RemovalEmployeeId = x.RemovalEmployeeId,
                            CustomerId = x.CustomerId,
                            InstallationSdkCaseId = x.InstallationSdkCaseId,
                            RemovalSdkCaseId = x.RemovalSdkCaseId,
                            RemovalFormId = x.RemovalFormId
                        }
                    ).ToListAsync();
                
                foreach (var item in list.Where(x => x.InstallationEmployeeId != null))
                {
                    if (item.Type == InstallationType.Installation)
                    {
                        var site = await core.SiteRead(item.InstallationEmployeeId.GetValueOrDefault());
                        item.AssignedTo = site.FirstName + " " + site.LastName;
                        if (item.InstallationSdkCaseId != null)
                        {
                            var sdkCaseId = (int) item.InstallationSdkCaseId;
                            var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                            if (caseLookup?.CheckUId != null && caseLookup?.CheckUId != 0)
                            {
                                item.InstallationSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                            }
                        }    
                    }
                }
                foreach (var item in list.Where(x => x.RemovalEmployeeId != null))
                {
                    if (item.Type == InstallationType.Removal)
                    {
                        var site = await core.SiteRead(item.RemovalEmployeeId.GetValueOrDefault());
                        item.AssignedTo = site.FirstName + " " + site.LastName;
                        if (item.RemovalSdkCaseId != null)
                        {
                            var sdkCaseId = (int) item.RemovalSdkCaseId;
                            var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                            if (caseLookup?.CheckUId != null && caseLookup?.CheckUId != 0)
                            {
                                item.RemovalSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                            }
                        }
                    }
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
                        CadastralNumber = customer.CadastralNumber,
                        ApartmentNumber = customer.ApartmentNumber != null ? customer.ApartmentNumber.ToString() : "",
                        PropertyNumber = customer.PropertyNumber != null ? customer.PropertyNumber.ToString() : "",
                        YearBuilt = customer.CompletionYear,
                        LivingFloorsNumber = customer.FloorsWithLivingSpace,
                        CustomerId = customer.Id,
                        CreatedByUserId = UserId,
                        UpdatedByUserId = UserId,
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

                        MainElement mainElement;

                        if (installation.Type == InstallationType.Installation)
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.InstallationFormId));
                            mainElement.Label = installation.CompanyName;
                            var dataElement = (DataElement) mainElement.ElementList[0];
                            dataElement.Label = installation.CompanyName;
                            dataElement.Description.InderValue = installation.CompanyAddress;
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CompanyAddress2)
                                ? ""
                                : $"<br>{installation.CompanyAddress2}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.ZipCode)
                                ? ""
                                : $"<br>{installation.ZipCode}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CityName)
                                ? ""
                                : $"<br>{installation.CityName}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CountryCode)
                                ? ""
                                : $"<br>{installation.CountryCode}";

                            var dataItem = (Text) dataElement.DataItemList[0];
                            dataItem.Value = installation.CadastralNumber;
                            
                            dataItem = (Text) dataElement.DataItemList[1];
                            dataItem.Value = installation.PropertyNumber;
                            
                            dataItem = (Text) dataElement.DataItemList[2];
                            dataItem.Value = installation.ApartmentNumber;

                            var dataItemSelect = (EntitySelect) dataElement.DataItemList[3];
                            
                            EntityGroupList model = await core.Advanced_EntityGroupAll(
                                "id", 
                                "eform-angular-installationchecking-plugin-editable-CadastralType",
                                0, 1, Constants.FieldTypes.EntitySelect,
                                false,
                                Constants.WorkflowStates.NotRemoved);

                            foreach (EntityItem entityItem in model.EntityGroups.First().EntityGroupItemLst)
                            {
                                if (entityItem.Name == installation.PropertyNumber)
                                {
                                    dataItemSelect.DefaultValue = entityItem.Id;
                                }
                            }
                            
                            var dataItemNumber = (Number) dataElement.DataItemList[4];
                            if (installation.YearBuilt != null)
                            {
                                dataItemNumber.DefaultValue = (int)installation.YearBuilt;
                            }
                            
                            dataItemNumber = (Number) dataElement.DataItemList[5];
                            if (installation.LivingFloorsNumber != null)
                            {
                                dataItemNumber.DefaultValue = (int)installation.LivingFloorsNumber;
                            }

                            mainElement.Repeated = 1;
                            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                            mainElement.StartDate = DateTime.Now.ToUniversalTime();
                            installation.InstallationEmployeeId = installationsAssignModel.EmployeeId;
                            installation.InstallationSdkCaseId = await core.CaseCreate(mainElement, "", installationsAssignModel.EmployeeId);
                            installation.State = InstallationState.Assigned;
                            installation.UpdatedByUserId = UserId;

                            await installation.Update(_installationCheckingContext);

                        }
                        else
                        {
                            mainElement = await core.TemplateRead(int.Parse(options.RemovalFormId));
                            mainElement.Label = installation.CompanyName;

                            var dataElement = (DataElement) mainElement.ElementList[0];
                            var removalDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                            dataElement.Label = installation.CompanyName;
                            dataElement.Description.InderValue = installation.CompanyAddress;
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CompanyAddress2)
                                ? ""
                                : $"<br>{installation.CompanyAddress2}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.ZipCode)
                                ? ""
                                : $"<br>{installation.ZipCode}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CityName)
                                ? ""
                                : $"<br>{installation.CityName}";
                            dataElement.Description.InderValue += string.IsNullOrEmpty(installation.CountryCode)
                                ? ""
                                : $"<br>{installation.CountryCode}";
                            dataElement.Description.InderValue += $"<br><b>Nedtagningsdato: {removalDate}</b>";
                            
                            EntityGroupList model = await core.Advanced_EntityGroupAll(
                                "id", 
                                $"eform-angular-installationchecking-plugin_{installation.Id}_hidden",
                                0, 1, Constants.FieldTypes.EntitySearch,
                                false,
                                Constants.WorkflowStates.NotRemoved);
                            
                            EntityGroup entityGroup;
                            
                            if (!model.EntityGroups.Any())
                            {
                                entityGroup = await core.EntityGroupCreate(
                                    Constants.FieldTypes.EntitySearch,
                                    $"eform-angular-installationchecking-plugin_{installation.Id}_hidden"
                                );
                            }
                            else
                            {
                                entityGroup = model.EntityGroups.First();
                            }

                            #region Image to PDF section
                            // Read image from file

                            try
                            {
                                string filename = installation.InstallationImageName.Replace(",", "");
                                string tempFilePath = Path.Combine("tmp", filename);
                                Directory.CreateDirectory("tmp");
                                Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: tempFilePath is {tempFilePath}");

                                if (core.GetSdkSetting(Settings.swiftEnabled).Result.ToLower() == "true")
                                {
                                    Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: swiftEnabled is true");
                                    var ss = await core.GetFileFromSwiftStorage(filename);
                                    var fileStream = File.Create(tempFilePath);
                                    ss.ObjectStreamContent.CopyTo(fileStream);
                                    fileStream.Close();
                                    fileStream.Dispose();

                                    ss.ObjectStreamContent.Close();
                                    ss.ObjectStreamContent.Dispose();
                                }
                                else
                                {
                                    if (core.GetSdkSetting(Settings.s3Enabled).Result.ToLower() == "true")
                                    {
                                        Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: s3Enabled is true");
                                        var ss = await core.GetFileFromS3Storage(filename);
                                        var fileStream = File.Create(tempFilePath);
                                        ss.ResponseStream.CopyTo(fileStream);
                                        fileStream.Close();
                                        fileStream.Dispose();

                                        ss.ResponseStream.Close();
                                        ss.ResponseStream.Dispose();
                                    }
                                }

                                using (MagickImage image = new MagickImage(tempFilePath))
                                {
                                    Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: MagickImage converting");
                                    // Create pdf file with a single page
                                    image.Write(tempFilePath
                                        .Replace("png", "pdf")
                                        .Replace("jpg", "pdf")
                                        .Replace("jpeg", "pdf"));
                                }

                                Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: Uploading PDF to Microting");
                                var resultId = await core.PdfUpload(tempFilePath
                                    .Replace("png", "pdf")
                                    .Replace("jpg", "pdf")
                                    .Replace("jpeg", "pdf"));

                                ShowPdf showPdf = (ShowPdf) dataElement.DataItemList[0];
                                showPdf.Value = resultId;
                                Log.LogEvent($"[DBG] InstallationsService.AssignInstallations: PDF set for field");

                            }
                            catch (Exception ex)
                            {
                                Log.LogException($"[ERR] InstallationsService.AssignInstallations convert image to pdf failed and got exception : {ex.Message}");
                            }
                            #endregion
                            
                            var i = 2;
                            foreach (var meter in installation.Meters.Where(x => !x.QR.IsNullOrEmpty()))
                            {
                                await core.EntitySearchItemCreate(
                                    entityGroup.Id,
                                    meter.QR,
                                    "",
                                    i.ToString()
                                );
                                EntitySearch entity = (EntitySearch)dataElement.DataItemList[i];
                                entity.EntityTypeId = int.Parse(entityGroup.MicrotingUUID);
                                entity.DisplayOrder = i;
                                i += 1;
                            }

                            int validFields = installation.Meters.Count(x => !x.QR.IsNullOrEmpty()) + 2;

                            try
                            {
                                for (int j = validFields; j < 52; j++)
                                {
                                    EntitySearch dataItem = (EntitySearch) dataElement.DataItemList[validFields];
                                    dataElement.DataItemList.Remove(dataItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.LogException(ex.Message);
                            }
                            
                            
                            installation.RemovalFormId = int.Parse(options.RemovalFormId);
                        
                            mainElement.Repeated = 1;
                            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                            mainElement.StartDate = DateTime.Now.ToUniversalTime();
                            installation.RemovalEmployeeId = installationsAssignModel.EmployeeId;
                            installation.RemovalSdkCaseId = await core.CaseCreate(mainElement, "", installationsAssignModel.EmployeeId);
                            installation.State = InstallationState.Assigned;
                            installation.UpdatedByUserId = UserId;

                            await installation.Update(_installationCheckingContext);
                        }
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

                    if (installation.Type == InstallationType.Installation)
                    {
                        await core.CaseDelete(installation.InstallationSdkCaseId.GetValueOrDefault());   


                        installation.InstallationEmployeeId = null;
                        installation.InstallationSdkCaseId = null;
                        installation.State = InstallationState.NotAssigned;
                        installation.UpdatedByUserId = UserId;
                        await installation.Update(_installationCheckingContext);

                        transaction.Commit();
                        return new OperationResult(true, _localizationService.GetString("InstallationRetractedSuccessfully"));                     
                    }
                    else
                    {
                        await core.CaseDelete(installation.RemovalSdkCaseId.GetValueOrDefault());


                        installation.RemovalEmployeeId = null;
                        installation.RemovalSdkCaseId = null;
                        installation.State = InstallationState.NotAssigned;
                        installation.UpdatedByUserId = UserId;
                        await installation.Update(_installationCheckingContext);

                        transaction.Commit();
                        return new OperationResult(true, _localizationService.GetString("InstallationRetractedSuccessfully"));
                    }
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

                var caseDto = await core.CaseLookupMUId(installation.InstallationSdkCaseId.GetValueOrDefault());

                if (caseDto == null)
                {
                    return new OperationDataResult<byte[]>(false, _localizationService.GetString("InstallationCaseNotFound"));
                }

                var assembly = Assembly.GetExecutingAssembly();
                var assemblyName = assembly.GetName().Name;

                using (var templateStream = assembly.GetManifestResourceStream($"{assemblyName}.Resources.export-template.xlsx"))
                using (var stream = new MemoryStream())
                {
                    var package = new ExcelPackage(stream, templateStream);
                
                    var worksheet = package.Workbook.Worksheets[0];
                    var row = 12;

                    foreach (var meter in installation.Meters.Where(x => !x.QR.IsNullOrEmpty()))
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
                        worksheet.Cells[row, 23].Value = installation.DateInstall?.ToString("dd-MM-yyyy");
                        worksheet.Cells[row, 23].Style.Numberformat.Format = "dd-MM-yyyy";
                        worksheet.Cells[row, 24].Value = installation.DateActRemove?.ToString("dd-MM-yyyy");
                        worksheet.Cells[row, 24].Style.Numberformat.Format = "dd-MM-yyyy";

                        var site = await core.SiteRead(installation.RemovalEmployeeId.GetValueOrDefault());
                        worksheet.Cells[row, 25].Value = site.FirstName + " " + site.LastName;

                        row++;
                    }
                    
                    package.Save();
                    package.Dispose();
                    var bytes = stream.ToArray();

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