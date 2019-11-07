using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;

namespace InstallationChecking.Pn.Infrastructure.Data.Seed.Data
{
    public class InstallationCheckingConfigurationSeedData : IPluginConfigurationSeedData
    {
        public PluginConfigurationValue[] Data => new[]
        {
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:MaxNumberOfWorkers",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:MaxParallelism",
                Value = "1"
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:SdkConnectionString",
                Value = "..."
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:InstallationFormId",
                Value = ""
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:RemovalFormId",
                Value = ""
            }
        };
    }
}