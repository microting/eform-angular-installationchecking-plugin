/*
The MIT License (MIT)

Copyright (c) 2007 - 2021 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace InstallationChecking.Pn.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using Infrastructure.Models;
    using Microsoft.EntityFrameworkCore;
    using Microting.InstallationCheckingBase.Infrastructure.Data;
    using Microting.eFormApi.BasePn.Abstractions;
    using Microting.eFormApi.BasePn.Infrastructure.Models.API;
    using Microting.eFormBaseCustomerBase.Infrastructure.Data;
    using Microting.InstallationCheckingBase.Infrastructure.Data.Entities;
    using Microting.InstallationCheckingBase.Infrastructure.Enums;
    using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
    using System.Reflection;
    using System.IO;
    using Microting.eForm.Dto;
    using Microting.eForm.Infrastructure.Models;
    using Microting.eFormApi.BasePn.Infrastructure.Helpers;
    using Microting.InstallationCheckingBase.Infrastructure.Models;
    using OpenStack.NetCoreSwiftClient.Extensions;
    using Microting.eForm.Infrastructure.Constants;
    using ClosedXML.Excel;
    using Microting.eFormApi.BasePn.Infrastructure.Models.Common;

    public class InstallationsService : IInstallationsService
    {
        private readonly IInstallationCheckingLocalizationService _localizationService;
        private readonly InstallationCheckingPnDbContext _installationCheckingContext;
        private readonly CustomersPnDbAnySql _customersContext;
        private readonly IPluginDbOptions<InstallationCheckingBaseSettings> _options;
        private readonly IEFormCoreService _coreHelper;
        private readonly IUserService _userService;

        public InstallationsService(
            InstallationCheckingPnDbContext installationCheckingContext,
            CustomersPnDbAnySql customersContext,
            IPluginDbOptions<InstallationCheckingBaseSettings> options,
            IInstallationCheckingLocalizationService localizationService,
            IUserService userService,
            IEFormCoreService coreHelper
        )
        {
            _installationCheckingContext = installationCheckingContext;
            _customersContext = customersContext;
            _options = options;
            _localizationService = localizationService;
            _userService = userService;
            _coreHelper = coreHelper;
        }

        public async Task<OperationDataResult<Paged<InstallationModel>>> Index(InstallationsRequestModel requestModel)
        {
            try
            {
                var core = await _coreHelper.GetCore();
                var options = _options.Value;
                var listModel = new Paged<InstallationModel>();

                var listQuery = _installationCheckingContext.Installations
                    .AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed);

                if (requestModel.State != null)
                {
                    listQuery = listQuery.Where(x => x.State == requestModel.State);
                }

                if (requestModel.Type != null)
                {
                    listQuery = listQuery.Where(x => x.Type == requestModel.Type);
                }

                if (!string.IsNullOrWhiteSpace(requestModel.NameFilter))
                {
                    listQuery = listQuery.Where(x => x.CompanyName.Contains(requestModel.NameFilter));
                }

                listQuery = QueryHelper.AddSortToQuery(listQuery, requestModel.Sort, requestModel.IsSortDsc);

                listModel.Total = await listQuery.Select(x => x.Id).CountAsync();

                listQuery = listQuery
                    .Skip(requestModel.Offset)
                    .Take(requestModel.PageSize);


                var list = await AddSelectToQuery(listQuery).ToListAsync();

                foreach (var item in list.Where(x => x.InstallationEmployeeId != null))
                {
                    item.InstallationFormId = int.Parse(options.InstallationFormId);
                    if (item.Type == InstallationType.Installation)
                    {
                        var site = await core.SiteRead(item.InstallationEmployeeId.GetValueOrDefault());
                        item.AssignedTo = site.FirstName + " " + site.LastName;
                        if (item.InstallationSdkCaseId != null)
                        {
                            var sdkCaseId = (int)item.InstallationSdkCaseId;
                            var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                            if (caseLookup?.CheckUId != null && caseLookup.CheckUId != 0)
                            {
                                item.InstallationSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                            }
                        }
                    }
                }
                foreach (var item in list.Where(x => x.RemovalEmployeeId != null))
                {
                    item.InstallationFormId = int.Parse(options.InstallationFormId);
                    if (item.Type == InstallationType.Removal)
                    {
                        var site = await core.SiteRead(item.RemovalEmployeeId.GetValueOrDefault());
                        item.AssignedTo = site.FirstName + " " + site.LastName;
                        if (item.RemovalSdkCaseId != null)
                        {
                            var sdkCaseId = (int)item.RemovalSdkCaseId;
                            var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                            if (caseLookup?.CheckUId != null && caseLookup?.CheckUId != 0)
                            {
                                item.RemovalSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                            }
                        }
                        if (item.InstallationSdkCaseId != null)
                        {
                            var sdkCaseId = (int)item.InstallationSdkCaseId;
                            var caseLookup = await core.CaseLookupMUId(sdkCaseId);
                            if (caseLookup?.CheckUId != null && caseLookup?.CheckUId != 0)
                            {
                                item.InstallationSdkCaseDbId = await core.CaseIdLookup(sdkCaseId, (int)caseLookup.CheckUId);
                            }
                        }
                    }
                }

                listModel.Entities = list;

                return new OperationDataResult<Paged<InstallationModel>>(true, listModel);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<Paged<InstallationModel>>(false, _localizationService.GetString("ErrorGettingInstallationsList"));
            }
        }

        public async Task<OperationResult> Create(int customerId)
        {
            await using var transaction = await _installationCheckingContext.Database.BeginTransactionAsync();
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
                    // CadastralType = customer.CadastralType != null ? customer.CadastralType.ToString() : "",
                    CustomerId = customer.Id,
                    CreatedByUserId = _userService.UserId,
                    UpdatedByUserId = _userService.UserId,
                };

                await installation.Create(_installationCheckingContext);

                await transaction.CommitAsync();
                return new OperationResult(true, _localizationService.GetString("InstallationCreatedSuccessfully"));
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Trace.TraceError(e.Message);
                return new OperationResult(false, _localizationService.GetString("ErrorWhileCreatingInstallation"));
            }
        }

        public async Task<OperationDataResult<InstallationModel>> Read(int id)
        {
            try
            {
                //var options = _options.Value;
                var query = _installationCheckingContext.Installations
                    .AsNoTracking()
                    .Where(x => x.WorkflowState != Constants.WorkflowStates.Removed && x.Id == id);

                var installationModel = await AddSelectToQuery(query).FirstOrDefaultAsync();

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
                            var language = await core.DbContextHelper.GetDbContext().Languages.SingleAsync(x => x.LanguageCode.ToLower() == "da");
                            mainElement = await core.ReadeForm(int.Parse(options.InstallationFormId), language);
                            mainElement.Label = installation.CompanyName;
                            var dataElement = (DataElement)mainElement.ElementList[0];
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

                            var dataItem = (Text)dataElement.DataItemList[0];
                            dataItem.Value = installation.CadastralNumber;

                            dataItem = (Text)dataElement.DataItemList[1];
                            dataItem.Value = installation.PropertyNumber;

                            dataItem = (Text)dataElement.DataItemList[2];
                            dataItem.Value = installation.ApartmentNumber;

                            var dataItemSelect = (EntitySelect)dataElement.DataItemList[3];

                            var model = await core.Advanced_EntityGroupAll(
                                "id",
                                "eform-angular-installationchecking-plugin-editable-CadastralType",
                                0, 1, Constants.FieldTypes.EntitySelect,
                                false,
                                Constants.WorkflowStates.NotRemoved);

                            foreach (var entityItem in model.EntityGroups.First().EntityGroupItemLst)
                            {
                                if (entityItem.Id == int.Parse(installation.CadastralType))
                                {
                                    dataItemSelect.DefaultValue = entityItem.Id;
                                }
                            }

                            var dataItemNumber = (Number)dataElement.DataItemList[4];
                            if (installation.YearBuilt != null)
                            {
                                dataItemNumber.DefaultValue = (int)installation.YearBuilt;
                            }

                            dataItemNumber = (Number)dataElement.DataItemList[5];
                            if (installation.LivingFloorsNumber != null)
                            {
                                dataItemNumber.DefaultValue = (int)installation.LivingFloorsNumber;
                            }

                            mainElement.Repeated = 1;
                            mainElement.EndDate = DateTime.Now.AddYears(10).ToUniversalTime();
                            mainElement.StartDate = DateTime.Now.ToUniversalTime();
                            installation.InstallationEmployeeId = installationsAssignModel.EmployeeId;
                            installation.InstallationSdkCaseId = await core.CaseCreate(mainElement, "", installationsAssignModel.EmployeeId, null);
                            installation.State = InstallationState.Assigned;
                            installation.UpdatedByUserId = _userService.UserId;

                            await installation.Update(_installationCheckingContext);

                        }
                        else
                        {
                            var language = await core.DbContextHelper.GetDbContext().Languages.SingleAsync(x => x.LanguageCode.ToLower() == "da");
                            mainElement = await core.ReadeForm(int.Parse(options.RemovalFormId), language);
                            mainElement.Label = installation.CompanyName;

                            var dataElement = (DataElement)mainElement.ElementList[0];
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

                            var model = await core.Advanced_EntityGroupAll(
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
                                    $"eform-angular-installationchecking-plugin_{installation.Id}_hidden",
                                    "");// TODO description is empty string
                            }
                            else
                            {
                                entityGroup = model.EntityGroups.First();
                            }

                            #region Image to PDF section
                            // Read image from file

                            try
                            {
                                var filename = installation.InstallationImageName.Replace(",", "");
                                var tempFilePath = Path.Combine("tmp", filename);
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

                                using (var image = new ImageMagick.MagickImage(tempFilePath))
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

                                var showPdf = (ShowPdf)dataElement.DataItemList[0];
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
                                var entity = (EntitySearch)dataElement.DataItemList[i];
                                entity.EntityTypeId = int.Parse(entityGroup.MicrotingUUID);
                                entity.DisplayOrder = i;
                                i += 1;
                            }

                            var validFields = installation.Meters.Count(x => !x.QR.IsNullOrEmpty()) + 2;

                            try
                            {
                                for (var j = validFields; j < 52; j++)
                                {
                                    var dataItem = (EntitySearch)dataElement.DataItemList[validFields];
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
                            installation.RemovalSdkCaseId = await core.CaseCreate(mainElement, "", installationsAssignModel.EmployeeId, null);
                            installation.State = InstallationState.Assigned;
                            installation.UpdatedByUserId = _userService.UserId;

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
                        installation.UpdatedByUserId = _userService.UserId;
                        await installation.Update(_installationCheckingContext);

                        transaction.Commit();
                        return new OperationResult(true, _localizationService.GetString("InstallationRetractedSuccessfully"));
                    }

                    await core.CaseDelete(installation.RemovalSdkCaseId.GetValueOrDefault());


                    installation.RemovalEmployeeId = null;
                    installation.RemovalSdkCaseId = null;
                    installation.State = InstallationState.NotAssigned;
                    installation.UpdatedByUserId = _userService.UserId;
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
                    installation.UpdatedByUserId = _userService.UserId;
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

                await using var templateStream =
                    assembly.GetManifestResourceStream($"{assemblyName}.Resources.export-template.xlsx");

                var timeStamp = $"{DateTime.UtcNow:yyyyMMdd}_{DateTime.UtcNow:hhmmss}";
                var resultDocument = Path.Combine(Path.GetTempPath(), "results", $"{timeStamp}_{installationId}.xlsx");
                await using var stream = new MemoryStream();

                var wb = new XLWorkbook(templateStream);

                var worksheet = wb.Worksheets.Add($"Data_{installationId}");
                var row = 12;

                foreach (var meter in installation.Meters.Where(x => !x.QR.IsNullOrEmpty()))
                {
                    worksheet.Cell(row, 1).Value = installation.CadastralNumber;
                    worksheet.Cell(row, 2).Value = installation.PropertyNumber;
                    worksheet.Cell(row, 3).Value = installation.CompanyName;
                    worksheet.Cell(row, 4).Value = installation.ApartmentNumber;
                    worksheet.Cell(row, 5).Value = installation.CompanyAddress;
                    worksheet.Cell(row, 6).Value = installation.CompanyAddress2;
                    worksheet.Cell(row, 7).Value = installation.ZipCode;
                    worksheet.Cell(row, 8).Value = installation.CityName;
                    worksheet.Cell(row, 9).Value = installation.CountryCode;
                    worksheet.Cell(row, 10).Value = installation.CadastralType;
                    worksheet.Cell(row, 11).Value = ""; // Foundation type
                    worksheet.Cell(row, 12).Value = ""; // Ventilation
                    worksheet.Cell(row, 13).Value = ""; // Household water
                    worksheet.Cell(row, 14).Value = installation.YearBuilt;
                    worksheet.Cell(row, 15).Value = ""; // Renovated
                    worksheet.Cell(row, 16).Value = ""; // Swedish "blue concrete"
                    worksheet.Cell(row, 17).Value = ""; // Visit by a professional
                    worksheet.Cell(row, 18).Value = installation.LivingFloorsNumber;
                    worksheet.Cell(row, 19).Value = meter.QR;
                    worksheet.Cell(row, 20).Value = meter.RoomType;
                    worksheet.Cell(row, 21).Value = meter.Floor;
                    worksheet.Cell(row, 22).Value = meter.RoomName;
                    worksheet.Cell(row, 23).Value = installation.DateInstall?.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 23).Style.NumberFormat.Format = "dd-MM-yyyy";
                    worksheet.Cell(row, 24).Value = installation.DateActRemove?.ToString("dd-MM-yyyy");
                    worksheet.Cell(row, 24).Style.NumberFormat.Format = "dd-MM-yyyy";

                    var site = await core.SiteRead(installation.RemovalEmployeeId.GetValueOrDefault());
                    worksheet.Cell(row, 25).Value = site.FirstName + " " + site.LastName;

                    row++;
                }

                wb.SaveAs(templateStream);
                wb.Dispose();

                Stream input = File.Open(resultDocument, FileMode.Open);

                var buffer = new byte[16 * 1024];
                await using var ms = new MemoryStream();
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                var bytes = ms.ToArray();
                await ms.DisposeAsync();

                return new OperationDataResult<byte[]>(true, bytes);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return new OperationDataResult<byte[]>(false, _localizationService.GetString("ErrorWhileExportingExcel"));
            }
        }

        private static IQueryable<InstallationModel> AddSelectToQuery(IQueryable<Installation> query)
        {
            return query.Select(x => new InstallationModel
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
            );
        }
    }
}