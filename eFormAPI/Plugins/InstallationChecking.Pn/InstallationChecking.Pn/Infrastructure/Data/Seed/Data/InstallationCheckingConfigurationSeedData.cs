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
                Name = "InstallationCheckingBaseSettings:LogLevel",
                Value = "4"
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:LogLimit",
                Value = "25000"
            },
            new PluginConfigurationValue()
            {
                Name = "InstallationCheckingBaseSettings:SdkConnectionString",
                Value = "..."
            }
        };
    }
}