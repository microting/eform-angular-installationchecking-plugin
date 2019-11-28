namespace InstallationChecking.Pn.Abstractions
{
    public interface IInstallationCheckingLocalizationService
    {
        string GetString(string key);
        string GetString(string format, params object[] args);
    }
}