﻿/*
The MIT License (MIT)

Copyright (c) 2007 - 2019 Microting A/S

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

using System;
using System.Collections.Generic;
using System.Reflection;
using InstallationChecking.Pn.Abstractions;
using InstallationChecking.Pn.Helpers;
using InstallationChecking.Pn.Infrastructure.Data.Seed;
using InstallationChecking.Pn.Infrastructure.Data.Seed.Data;
using InstallationChecking.Pn.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microting.InstallationCheckingBase.Infrastructure.Data;
using Microting.InstallationCheckingBase.Infrastructure.Data.Factories;
using Microting.eFormApi.BasePn;
using Microting.eFormApi.BasePn.Infrastructure.Database.Extensions;
using Microting.eFormApi.BasePn.Infrastructure.Models.Application;
using Microting.eFormApi.BasePn.Infrastructure.Settings;
using Microting.eFormApi.BasePn.Infrastructure.Helpers;
using Microting.InstallationCheckingBase.Infrastructure.Const;
using Microting.eFormApi.BasePn.Abstractions;
using Microting.eFormApi.BasePn.Infrastructure.Helpers.PluginDbOptions;
using Microting.eFormBaseCustomerBase.Infrastructure.Data;
using Microting.InstallationCheckingBase.Infrastructure.Models;

namespace InstallationChecking.Pn
{
    using Microting.eFormApi.BasePn.Infrastructure.Consts;
    using Microting.eFormApi.BasePn.Infrastructure.Models.Application.NavigationMenu;

    public class EformInstallationCheckingPlugin : IEformPlugin
    {
        public string Name => "Microting InstallationChecking Plugin";
        public string PluginId => "eform-angular-installationchecking-plugin";
        public string PluginPath => PluginAssembly().Location;
        public string PluginBaseUrl => "installationchecking-pn";

        public Assembly PluginAssembly()
        {
            return typeof(EformInstallationCheckingPlugin).GetTypeInfo().Assembly;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            // Do nothing
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IInstallationCheckingLocalizationService, InstallationCheckingLocalizationService>();
            services.AddTransient<IInstallationCheckingPnSettingsService, InstallationCheckingPnSettingsService>();
            services.AddTransient<IInstallationsService, InstallationsService>();

            SeedInstallationForms(services);
        }

        public void ConfigureOptionsServices(IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigurePluginDbOptions<InstallationCheckingBaseSettings>(
                configuration.GetSection("InstallationCheckingBaseSettings"));
        }

        public void ConfigureDbContext(IServiceCollection services, string connectionString)
        {
            string customersConnectionString = connectionString.Replace(
                "eform-angular-installationchecking-plugin",
                "eform-angular-basecustomer-plugin");
            services.AddDbContext<InstallationCheckingPnDbContext>(o =>
                o.UseMySql(connectionString, new MariaDbServerVersion(
                new Version(10, 4, 0)), mySqlOptionsAction: builder =>
            {
                builder.EnableRetryOnFailure();
                builder.MigrationsAssembly(PluginAssembly().FullName);
            }));

            services.AddDbContext<CustomersPnDbAnySql>(o =>
                o.UseMySql(connectionString, new MariaDbServerVersion(
                new Version(10, 4, 0)), mySqlOptionsAction: builder =>
            {
                builder.EnableRetryOnFailure();
                builder.MigrationsAssembly(PluginAssembly().FullName);
            }));

            InstallationCheckingPnContextFactory contextFactory = new InstallationCheckingPnContextFactory();
            var context = contextFactory.CreateDbContext(new[] {connectionString});
            context.Database.Migrate();

            // Seed database
            SeedDatabase(connectionString);
        }

        public List<PluginMenuItemModel> GetNavigationMenu(IServiceProvider serviceProvider)
        {
            var pluginMenu = new List<PluginMenuItemModel>()
                {
                    new PluginMenuItemModel
                    {
                        Name = "Dropdown",
                        E2EId = "installationchecking",
                        Link = "",
                        Type = MenuItemTypeEnum.Dropdown,
                        Position = 0,
                        Translations = new List<PluginMenuTranslationModel>()
                        {
                            new PluginMenuTranslationModel
                            {
                                 LocaleName = LocaleNames.English,
                                 Name = "Installation checking",
                                 Language = LanguageNames.English,
                            },
                            new PluginMenuTranslationModel
                            {
                                 LocaleName = LocaleNames.German,
                                 Name = "Installation checking",
                                 Language = LanguageNames.German,
                            },
                            new PluginMenuTranslationModel
                            {
                                 LocaleName = LocaleNames.Danish,
                                 Name = "Installation checking",
                                 Language = LanguageNames.Danish,
                            }
                        },
                        ChildItems = new List<PluginMenuItemModel>()
                        {
                            new PluginMenuItemModel
                            {
                                Name = "Installation",
                                E2EId = "installationchecking-pn-installation",
                                Link = "/plugins/installationchecking-pn/installation",
                                Type = MenuItemTypeEnum.Link,
                                Position = 0,
                                MenuTemplate = new PluginMenuTemplateModel()
                                {
                                    Name = "Installation",
                                    E2EId = "installationchecking-pn-installation",
                                    DefaultLink = "/plugins/installationchecking-pn/installation",
                                    Permissions = new List<PluginMenuTemplatePermissionModel>(),
                                    Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Installation",
                                            Language = LanguageNames.English,
                                        },
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Installation",
                                            Language = LanguageNames.German,
                                        },
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Installation",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                                },
                                Translations = new List<PluginMenuTranslationModel>
                                {
                                    new PluginMenuTranslationModel
                                {
                                    LocaleName = LocaleNames.English,
                                    Name = "Installation",
                                    Language = LanguageNames.English,
                                },
                                    new PluginMenuTranslationModel
                                {
                                    LocaleName = LocaleNames.German,
                                    Name = "Installation",
                                    Language = LanguageNames.German,
                                },
                                    new PluginMenuTranslationModel
                                {
                                    LocaleName = LocaleNames.Danish,
                                    Name = "Installation",
                                    Language = LanguageNames.Danish,
                                },
                                }
                            },
                            new PluginMenuItemModel
                            {
                                Name = "Removal",
                                E2EId = "installationchecking-pn-removal",
                                Link = "/plugins/installationchecking-pn/removal",
                                Type = MenuItemTypeEnum.Link,
                                Position = 1,
                                MenuTemplate = new PluginMenuTemplateModel()
                                {
                                    Name = "Removal",
                                    E2EId = "items-planning-pn-reports",
                                    DefaultLink = "/plugins/items-planning-pn/reports",
                                    Permissions = new List<PluginMenuTemplatePermissionModel>(),
                                    Translations = new List<PluginMenuTranslationModel>
                                    {
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.English,
                                            Name = "Removal",
                                            Language = LanguageNames.English,
                                        },
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.German,
                                            Name = "Removal",
                                            Language = LanguageNames.German,
                                        },
                                        new PluginMenuTranslationModel
                                        {
                                            LocaleName = LocaleNames.Danish,
                                            Name = "Fjernelse",
                                            Language = LanguageNames.Danish,
                                        },
                                    }
                                },
                                Translations = new List<PluginMenuTranslationModel>
                                {
                                    new PluginMenuTranslationModel
                                    {
                                        LocaleName = LocaleNames.English,
                                        Name = "Removal",
                                        Language = LanguageNames.English,
                                    },
                                    new PluginMenuTranslationModel
                                    {
                                        LocaleName = LocaleNames.German,
                                        Name = "Removal",
                                        Language = LanguageNames.German,
                                    },
                                    new PluginMenuTranslationModel
                                    {
                                        LocaleName = LocaleNames.Danish,
                                        Name = "Fjernelse",
                                        Language = LanguageNames.Danish,
                                    },
                                }
                            }
                        }
                    }
                };

            return pluginMenu;
        }

        public MenuModel HeaderMenu(IServiceProvider serviceProvider)
        {
            var localizationService = serviceProvider
                .GetService<IInstallationCheckingLocalizationService>();

            var result = new MenuModel();
            result.LeftMenu.Add(new MenuItemModel()
            {
                Name = localizationService.GetString("Planning"),
                E2EId = "installationchecking",
                Link = "",
                Guards = new List<string>() { InstallationCheckingClaims.AccessInstallationCheckingPlugin },
                MenuItems = new List<MenuItemModel>()
                {
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Installation"),
                        E2EId = "installationchecking-pn-installation",
                        Link = "/plugins/installationchecking-pn/installation",
                        Position = 0,
                    },
                    new MenuItemModel()
                    {
                        Name = localizationService.GetString("Removal"),
                        E2EId = "installationchecking-pn-removal",
                        Link = "/plugins/installationchecking-pn/removal",
                        Position = 1,
                    }
                }
            });
            return result;
        }

        public void SeedDatabase(string connectionString)
        {
            var contextFactory = new InstallationCheckingPnContextFactory();
            using (var context = contextFactory.CreateDbContext(new []{connectionString}))
            {
                InstallationCheckingPluginSeed.SeedData(context);
            }
        }

        public void AddPluginConfig(IConfigurationBuilder builder, string connectionString)
        {
            var seedData = new InstallationCheckingConfigurationSeedData();
            var contextFactory = new InstallationCheckingPnContextFactory();
            builder.AddPluginConfiguration(
                connectionString,
                seedData,
                contextFactory);
        }

        public PluginPermissionsManager GetPermissionsManager(string connectionString)
        {
            var contextFactory = new InstallationCheckingPnContextFactory();
            var context = contextFactory.CreateDbContext(new[] { connectionString });

            return new PluginPermissionsManager(context);
        }

        private async void SeedInstallationForms(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var pluginDbOptions = serviceProvider.GetRequiredService<IPluginDbOptions<InstallationCheckingBaseSettings>>();

            var core = await serviceProvider.GetRequiredService<IEFormCoreService>().GetCore();
            var context = serviceProvider.GetRequiredService<InstallationCheckingPnDbContext>();

            if (string.IsNullOrEmpty(pluginDbOptions.Value.InstallationFormId))
            {
                var installationFormId = await SeedHelper.CreateInstallationForm(core);
                await pluginDbOptions.UpdateDb(settings => settings.InstallationFormId = installationFormId.ToString(), context, 1);
            }

            if (string.IsNullOrEmpty(pluginDbOptions.Value.RemovalFormId))
            {
                var removalFormId = await SeedHelper.CreateRemovalForm(core);
                await pluginDbOptions.UpdateDb(settings => settings.RemovalFormId = removalFormId.ToString(), context, 1);
            }
        }
    }
}
