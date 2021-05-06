using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Microting.InstallationCheckingBase.Infrastructure.Data;
using Microting.InstallationCheckingBase.Infrastructure.Data.Factories;

namespace InstallationChecking.Pn.Test
{
    [TestFixture]
    public abstract class DbTestFixture
    {
        protected InstallationCheckingPnDbContext DbContext;
        private string _connectionString;

        private void GetContext(string connectionStr)
        {
            InstallationCheckingPnContextFactory contextFactory = new InstallationCheckingPnContextFactory();
            DbContext = contextFactory.CreateDbContext(new[] {connectionStr});

            DbContext.Database.Migrate();
            DbContext.Database.EnsureCreated();
        }

        [SetUp]
        public void Setup()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _connectionString = @"data source=(LocalDb)\SharedInstance;Initial catalog=installationchecking-base-tests;Integrated Security=true";
            }
            else
            {
                _connectionString = @"Server = localhost; port = 3306; Database = installationchecking-base-tests; user = root; Convert Zero Datetime = true;";
            }

            GetContext(_connectionString);

            DbContext.Database.SetCommandTimeout(300);

            try
            {
                ClearDb();
            }
            catch
            {
                DbContext.Database.Migrate();
            }
            DoSetup();
        }

        [TearDown]
        public void TearDown()
        {
            ClearDb();

            ClearFile();

            DbContext.Dispose();
        }

        private void ClearDb()
        {
            List<string> modelNames = new List<string>();
            modelNames.Add("Installations");
            modelNames.Add("InstallationVersions");
            modelNames.Add("PluginConfigurationValues");
            modelNames.Add("PluginConfigurationValueVersions");
            modelNames.Add("PluginPermissions");
            modelNames.Add("PluginGroupPermissions");
            modelNames.Add("PluginGroupPermissionVersions");

            foreach (var modelName in modelNames)
            {
                try
                {
                    string sqlCmd;
                    if (DbContext.Database.IsMySql())
                    {
                        sqlCmd = $"SET FOREIGN_KEY_CHECKS = 0;TRUNCATE `installationchecking-base-tests`.`{modelName}`";
                    }
                    else
                    {
                        sqlCmd = $"DELETE FROM [{modelName}]";
                    }
                    DbContext.Database.ExecuteSqlRaw(sqlCmd);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private string path;

        private void ClearFile()
        {
            path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            path = Path.GetDirectoryName(path).Replace(@"file:\", "");

            string picturePath = path + @"\output\dataFolder\picture\Deleted";

            DirectoryInfo diPic = new DirectoryInfo(picturePath);

            try
            {
                foreach (FileInfo file in diPic.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }
        }

        protected virtual void DoSetup() { }
    }
}