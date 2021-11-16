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

namespace InstallationChecking.Pn.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using eFormCore;
    using Microsoft.EntityFrameworkCore;
    using Microting.eForm.Infrastructure.Constants;
    using Microting.eForm.Infrastructure.Models;
    using EntityGroup = Microting.eForm.Infrastructure.Data.Entities.EntityGroup;

    public class SeedHelper
    {
        private static async Task<int> CreateCadastralTypeList(Core core)
        {
            var model = await core.Advanced_EntityGroupAll(
                "id", 
                "eform-angular-installationchecking-plugin-editable-CadastralType",
                0, 1, Constants.FieldTypes.EntitySelect,
                false,
                Constants.WorkflowStates.NotRemoved);

            EntityGroup group;
            
            if (!model.EntityGroups.Any())
            {
                group = await core.EntityGroupCreate(Constants.FieldTypes.EntitySelect,
                    "eform-angular-installationchecking-plugin-editable-CadastralType",
                    "", false, false);// TODO description is empty string

                var roomTypes = new List<string>()
                {
                    "Arbejdsplads", "Boligblok", "Boligbyggeri", "Butikker", "Delvist fritliggende hus", "Feriehus",
                    "Fritliggende hus", "Generelle faciliteter", "Hospital", "Hotel", "Hus i klippe",
                    "Information mangler", "Kaserne", "Kirke", "Kraftværk", "Plejehjem", "Rækkehus",
                    "Rækkehuse forbundet med garage", "Skole/institution", "Slot", "Stort hus", "Vandværk"
                };

                const int i = 0;
                foreach (var roomType in roomTypes)
                {
                    await core.EntitySelectItemCreate(group.Id,roomType,i,i.ToString());
                }
            }
            else
            {
                group = model.EntityGroups.First();
            }

            return int.Parse(group.MicrotingUid);
        }
        
        private static async Task<int> CreateRoomTypeList(Core core)
        {
            var model = await core.Advanced_EntityGroupAll(
                "id", 
                "eform-angular-installationchecking-plugin-editable-RoomType",
                0, 1, Constants.FieldTypes.EntitySelect,
                false,
                Constants.WorkflowStates.NotRemoved);

            EntityGroup group;
            
            if (!model.EntityGroups.Any())
            {
                group = await core.EntityGroupCreate(Constants.FieldTypes.EntitySelect, 
                    "eform-angular-installationchecking-plugin-editable-RoomType",
                    "", false, false);// TODO description is empty string

                var roomTypes = new List<string>()
                {
                    "Soveværelse","Stue","Hobbyrum","Gang","Køkken","Kontor","Kælder","Andre lukkede rum"
                };

                var i = 0;
                foreach (var roomType in roomTypes)
                {
                    await core.EntitySelectItemCreate(group.Id,roomType,i,i.ToString());
                }
            }
            else
            {
                group = model.EntityGroups.First();
            }

            return int.Parse(group.MicrotingUid);
        }

        public static async Task<int> CreateInstallationForm(Core core)
        {
            const string timeZone = "Europe/Copenhagen";
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
            }
            var language = await core.DbContextHelper.GetDbContext().Languages.SingleAsync(x => x.LanguageCode.ToLower() == "da");
            var roomTypeListId = await CreateRoomTypeList(core);
            var cadastralTypeId = await CreateCadastralTypeList(core);
            var templatesDto = await core.TemplateItemReadAll(false,
                "",
                "eform-angular-installationchecking-plugin-installation",
                false,
                "",
                new List<int>(),
                timeZoneInfo,
                language
                );

            if (templatesDto.Count > 0)
            {
                return templatesDto.First().Id;
            }
            else
            {
                var installationForm = new MainElement
                {
                    Id = 141699,
                    Repeated = 0,
                    Label = "eform-angular-installationchecking-plugin-installation",
                    StartDate = new DateTime(2019, 11, 4),
                    EndDate = new DateTime(2029, 11, 4),
                    Language = "da",
                    MultiApproval = false,
                    FastNavigation = false,
                    DisplayOrder = 0,
                };
            
                var dataItems = new List<DataItem>();

                dataItems.Add(new Text(
                        1,
                        false,
                        false,
                        "Matrikelnummer",
                        "",
                        "e8eaf6",
                        0,
                        false,
                        "",
                        0,
                        false,
                        false,
                        false,
                        false,
                        ""
                    )
                );
                dataItems.Add(new Text(
                        2,
                        false,
                        false,
                        "Ejendomsnummer",
                        "",
                        "e8eaf6",
                        1,
                        false,
                        "",
                        0,
                        false,
                        false,
                        false,
                        false,
                        ""
                    )
                );
                dataItems.Add(new Text(
                        3,
                        false,
                        false,
                        "Lejlighedsnummer",
                        "",
                        "e8eaf6",
                        2,
                        false,
                        "",
                        0,
                        false,
                        false,
                        false,
                        false,
                        ""
                    )
                );
                // TODO seed this as a EntitySelect group DONE
                dataItems.Add(new EntitySelect(1, 
                        true, 
                        false, 
                        "Matrikeltype *", 
                        "", 
                        "ffe4e4",
                        3, 
                        false, 0, 
                        cadastralTypeId
                    )
                );
                dataItems.Add(new Number(
                        5,
                        false,
                        false,
                        "Opførselsår",
                        "",
                        "e8eaf6",
                        4,
                        false,
                        "",
                        "",
                        0,
                        0,
                        ""
                    )
                );
                dataItems.Add(new Number(
                        6,
                        true,
                        false,
                        "Antal etager med beboelse *",
                        "<u>Forklaring:<br></u>Vær opmærksom på at det er antallet af beboede etager, ikke det totale antal etager",
                        "ffe4e4",
                        5,
                        false,
                        "",
                        "",
                        0,
                        0,
                        ""
                    )
                );
                dataItems.Add(new Picture(
                        7,
                        false,
                        false,
                        "Tag evt. billede af kort/tegning over placeringer, når alle målere er opsat *",
                        "<br>",
                        "e8eaf6",
                        6,
                        false,
                        1,
                        false
                    )
                );
                dataItems.Add(new SaveButton(
                        8,
                        true,
                        false,
                        "Tryk GEM DATA, når alle målere er opsat og dokumenteret",
                        "",
                        "f0f8db",
                        7,
                        false,
                        ""
                    )
                );

                for (var i = 1; i <= 50; i++)
                {
                    var inc = (i - 1) * 5;
                    var color = (inc % 2 == 0) ? "fff6df" : "e8eaf6";
                    
                    dataItems.Add(new Text(
                            inc + 9,
                            false,
                            false,
                            $"Måler {i} - QR",
                            "",
                            color,
                            inc + 8,
                            false,
                            "",
                            0,
                            false,
                            false,
                            false,
                            true,
                            Constants.BarcodeTypes.QrCode
                        )
                    );
                    
                    dataItems.Add(new Picture(
                        inc + 10, 
                        false, 
                        false, 
                        $"Måler {i} - Billed", 
                        "", 
                        color, 
                        inc + 9, 
                        false, 
                        1, 
                        true));
                    dataItems.Add(new EntitySelect(
                        inc + 11, 
                        false, 
                        false, 
                        $"Måler {i} - Rumtype", 
                        "", 
                        color,
                        inc + 10, 
                        false, 0, 
                        roomTypeListId)
                    );
                    dataItems.Add(new Number(
                            inc + 12,
                            false,
                            false,
                            $"Måler {i} - Etage",
                            "<u>Forklaring:<br></u>Etage for lokalet som måleren placeres i. Forskudt/kælderplan = 0, stueetage = 1 osv.",
                            color,
                            inc + 11,
                            false,
                            "",
                            "",
                            0,
                            0,
                            ""
                        )
                    );
                    dataItems.Add(new Text(
                            inc + 13,
                            false,
                            false,
                            $"Måler {i} - Rumnavn",
                            "",
                            color,
                            inc + 12,
                            false,
                            "",
                            0,
                            false,
                            false,
                            false,
                            false,
                            ""
                        )
                    );
                }
            
                dataItems.Add(new SaveButton(
                        259,
                        false,
                        false,
                        "Tryk GEM DATA, når alle målere er opsat og dokumenteret",
                        "",
                        "f0f8db",
                        999,
                        false,
                        "GEM DATA"
                    )
                );

                var dataElement = new DataElement(
                    141704,
                    "CompanyName",
                    0,
                    "CompanyAddress<br>CompanyAddress2<br>ZipCode<br>CityName<br>Country",
                    false,
                    false,
                    false,
                    false,
                    "",
                    false,
                    new List<DataItemGroup>(),
                    dataItems);

                installationForm.ElementList.Add(dataElement);
            
                installationForm = await core.TemplateUploadData(installationForm);
                return await core.TemplateCreate(installationForm);
            }
        }

        public static async Task<int> CreateRemovalForm(Core core)
        {
            var timeZone = "Europe/Copenhagen";
            TimeZoneInfo timeZoneInfo;
            try
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");
            }
            var language = await core.DbContextHelper.GetDbContext().Languages.SingleAsync(x => x.LanguageCode.ToLower() == "da");
            var templatesDto = await core.TemplateItemReadAll(false,
                "",
                "eform-angular-installationchecking-plugin-removal",
                false,
                "",
                new List<int>(),
                timeZoneInfo,
                language
            );

            if (templatesDto.Count > 0)
            {
                return templatesDto.First().Id;
            }
            else
            {
                var entityGroup = await core.EntityGroupCreate(
                    Constants.FieldTypes.EntitySearch,
                    $"eform-angular-installationchecking-plugin_0",
                    "", false, false);// TODO description is empty string
                
                
                var removalForm = new MainElement
                {
                    Id = 141709,
                    Repeated = 0,
                    Label = "eform-angular-installationchecking-plugin-removal",
                    StartDate = new DateTime(2019, 11, 4),
                    EndDate = new DateTime(2029, 11, 4),
                    Language = "da",
                    MultiApproval = false,
                    FastNavigation = false,
                    DisplayOrder = 0,
                };

                var dataItems = new List<DataItem>();
            
                dataItems.Add(new ShowPdf(
                    1,
                    true,
                    false,
                    "Vis PDF med kort over placering af målere",
                    "Det kan være en fordel at udskrive oversigten, før nedtagningsadresser besøges.",
                    "e8eaf6",
                    0,
                    false,
                    "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf"
                ));
                dataItems.Add(new SaveButton(
                    2,
                    true,
                    false,
                    "Tryk GEM DATA, når alle målere er QR-scannet",
                    "",
                    "f0f8db",
                    1,
                    false,
                    "GEM DATA"
                ));
                for (var i = 1; i < 51; i++)
                {
                    dataItems.Add(new EntitySearch(
                        3 + i,
                        false,
                        false,
                        $"Måler {i} - QR",
                        "",
                        "e8eaf6",
                        i,
                        false,
                        0,
                        int.Parse(entityGroup.MicrotingUid),
                        false,
                        "",
                        3,
                        true,
                        Constants.BarcodeTypes.QrCode)
                    );
                }
                dataItems.Add(new SaveButton(
                    51,
                    true,
                    false,
                    "Tryk GEM DATA, når alle målere er QR-scannet",
                    "",
                    "f0f8db",
                    999,
                    false,
                    "GEM DATA"
                ));

                var dataElement = new DataElement(
                    141714,
                    "CompanyName",
                    0,
                    "CompanyAddress<br>CompanyAddress2<br>ZipCode<br>CityName<br>Country<br><b>Nedtagningsdato: 2020-02-23</b>",
                    false,
                    false,
                    false,
                    false,
                    "",
                    false,
                    new List<DataItemGroup>(),
                    dataItems);

                removalForm.ElementList.Add(dataElement);
                removalForm = await core.TemplateUploadData(removalForm);
            
                return await core.TemplateCreate(removalForm);
            }
        }
    }
}