using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstallationChecking.Pn.Abstractions;
using InstallationChecking.Pn.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microting.InstallationCheckingBase.Infrastructure.Data;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormApi.BasePn.Infrastructure.Models.API;

namespace InstallationChecking.Pn.Services
{
    public class InstallationCheckingPnSettingsService :IInstallationCheckingPnSettingsService
    {
        private readonly ILogger<InstallationCheckingPnSettingsService> _logger;
        private readonly IInstallationCheckingLocalizationService _trashInspectionLocalizationService;
        private readonly InstallationCheckingPnDbContext _dbContext;
        private readonly IPluginDbOptions<InstallationCheckingBaseSettings> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public InstallationCheckingPnSettingsService(ILogger<InstallationCheckingPnSettingsService> logger,
            IInstallationCheckingLocalizationService trashInspectionLocalizationService,
            InstallationCheckingPnDbContext dbContext,
            IPluginDbOptions<InstallationCheckingBaseSettings> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _options = options;
            _httpContextAccessor = httpContextAccessor;
            _trashInspectionLocalizationService = trashInspectionLocalizationService;
        }
        
        public async Task<OperationDataResult<InstallationCheckingBaseSettings>> GetSettings()
        {
            try
            {
                var option = _options.Value;

                if (option.SdkConnectionString == "...")
                {
                    string connectionString = _dbContext.Database.GetDbConnection().ConnectionString;

                    string dbNameSection = Regex.Match(connectionString, @"(Database=(...)_eform-angular-\w*-plugin;)").Groups[0].Value;
                    string dbPrefix = Regex.Match(connectionString, @"Database=(\d*)_").Groups[1].Value;
                    string sdk = $"Database={dbPrefix}_SDK;";
                    connectionString = connectionString.Replace(dbNameSection, sdk);
                    await _options.UpdateDb(settings => { settings.SdkConnectionString = connectionString;}, _dbContext, UserId);
                }

                return new OperationDataResult<InstallationCheckingBaseSettings>(true, option);
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationDataResult<InstallationCheckingBaseSettings>(false,
                    _trashInspectionLocalizationService.GetString("ErrorWhileObtainingTrashInspectionSettings"));
            }
        }

        public async Task<OperationResult> UpdateSettings(InstallationCheckingBaseSettings installationcheckingBaseSettings)
        {
            try
            {
                await _options.UpdateDb(settings =>
                {
                    settings.LogLevel = installationcheckingBaseSettings.LogLevel;
                    settings.LogLimit = installationcheckingBaseSettings.LogLimit;
                    settings.SdkConnectionString = installationcheckingBaseSettings.SdkConnectionString;
                }, _dbContext, UserId);

                return new OperationResult(true,
                    _trashInspectionLocalizationService.GetString("SettingsHaveBeenUpdatedSuccessfully"));
            }
            catch(Exception e)
            {
                Trace.TraceError(e.Message);
                _logger.LogError(e.Message);
                return new OperationResult(false, _trashInspectionLocalizationService.GetString("ErrorWhileUpdatingSettings"));
            }
        }
        
        public int UserId
        {
            get
            {
                var value = _httpContextAccessor?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return value == null ? 0 : int.Parse(value);
            }
        }
    }
}