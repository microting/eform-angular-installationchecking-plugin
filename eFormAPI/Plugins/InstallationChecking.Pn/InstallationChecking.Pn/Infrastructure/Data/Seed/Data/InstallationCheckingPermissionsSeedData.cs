using Microting.eFormApi.BasePn.Infrastructure.Database.Entities;
using Microting.InstallationCheckingBase.Infrastructure.Const;

namespace InstallationChecking.Pn.Infrastructure.Data.Seed.Data
{
    public static class InstallationCheckingPermissionsSeedData
    {
        public static PluginPermission[] Data => new[]
        {
            new PluginPermission()
            {
                PermissionName = "Access InstallationChecking Plugin",
                ClaimName = InstallationCheckingClaims.AccessInstallationCheckingPlugin
            },
            new PluginPermission()
            {
                PermissionName = "Create Installations",
                ClaimName = InstallationCheckingClaims.CreateInstallations
            },
            new PluginPermission()
            {
                PermissionName = "Assign Installations",
                ClaimName = InstallationCheckingClaims.AssignInstallations
            },
            new PluginPermission()
            {
                PermissionName = "Archive Installations",
                ClaimName = InstallationCheckingClaims.ArchiveInstallations
            },
            new PluginPermission()
            {
                PermissionName = "Export Installations",
                ClaimName = InstallationCheckingClaims.ExportInstallations
            }
        };
    }
}