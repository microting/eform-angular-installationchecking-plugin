using InstallationChecking.Pn.Abstractions;
using Microsoft.Extensions.Localization;
using Microting.eFormApi.BasePn.Localization.Abstractions;

namespace InstallationChecking.Pn.Services
{
    public class InstallationCheckingLocalizationService :IInstallationCheckingLocalizationService
    {
        private readonly IStringLocalizer _localizer;
        
        // ReSharper disable once SuggestBaseTypeForParameter
        public InstallationCheckingLocalizationService(IEformLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(EformInstallationCheckingPlugin));
        }
        
        public string GetString(string key)
        {
            var str = _localizer[key];
            return str.Value;
        }

        public string GetString(string format, params object[] args)
        {
            var message = _localizer[format];
            if (message?.Value == null)
            {
                return null;
            }

            return string.Format(message.Value, args);
        }
    }
}