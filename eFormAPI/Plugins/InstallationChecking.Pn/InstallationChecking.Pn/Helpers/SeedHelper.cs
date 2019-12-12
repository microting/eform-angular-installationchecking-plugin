using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eFormCore;
using Microting.eForm.Infrastructure.Constants;
using Microting.eForm.Infrastructure.Models;
using KeyValuePair = Microting.eForm.Dto.KeyValuePair;

namespace InstallationChecking.Pn.Helpers
{
    public class SeedHelper
    {
        public static async Task<int> CreateInstallationForm(Core core)
        {
            var templatesDto = await core.TemplateItemReadAll(false,
                "",
                "Radonmålinger Opsætning",
                false,
                "",
                null);

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
                Label = "Radonmålinger Opsætning",
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
            // TODO seed this as a EntitySelect group
            dataItems.Add(new SingleSelect(
                    4,
                    true,
                    false,
                    "Matrikeltype *",
                    "",
                    "e8eaf6",
                    3,
                    false,
                    new List<KeyValuePair>
                    {
                        new KeyValuePair(
                            "1", 
                            "Arbejdsplads", 
                            false, 
                            "1"
                        ),
                        new KeyValuePair(
                            "2", 
                            "Boligblok", 
                            false, 
                            "2"
                        ),
                        new KeyValuePair(
                            "3", 
                            "Boligbyggeri", 
                            false, 
                            "3"
                        ),
                        new KeyValuePair(
                            "4", 
                            "Butikker", 
                            false, 
                            "4"
                        ),
                        new KeyValuePair(
                            "5", 
                            "Delvist fritliggende hus", 
                            false, 
                            "5"
                        ),
                        new KeyValuePair(
                            "6", 
                            "Feriehus", 
                            false, 
                            "6"
                        ),
                        new KeyValuePair(
                            "7", 
                            "Fritliggende hus", 
                            false, 
                            "7"
                        ),
                        new KeyValuePair(
                            "8", 
                            "Generelle faciliteter", 
                            false, 
                            "8"
                        ),
                        new KeyValuePair(
                            "9", 
                            "Hospital", 
                            false, 
                            "9"
                        ),
                        new KeyValuePair(
                            "10", 
                            "Hotel", 
                            false, 
                            "10"
                        ),
                        new KeyValuePair(
                            "11", 
                            "Hus i klippe", 
                            false, 
                            "11"
                        ),
                        new KeyValuePair(
                            "12", 
                            "Information mangler", 
                            false, 
                            "12"
                        ),
                        new KeyValuePair(
                            "13", 
                            "Kaserne", 
                            false, 
                            "13"
                        ),
                        new KeyValuePair(
                            "14", 
                            "Kirke", 
                            false, 
                            "14"
                        ),
                        new KeyValuePair(
                            "15", 
                            "Kraftværk", 
                            false, 
                            "15"
                        ),
                        new KeyValuePair(
                            "16", 
                            "Plejehjem", 
                            false, 
                            "16"
                        ),
                        new KeyValuePair(
                            "17", 
                            "Rækkehus", 
                            false, 
                            "17"
                        ),
                        new KeyValuePair(
                            "18", 
                            "Rækkehuse forbundet med garage", 
                            false, 
                            "18"
                        ),
                        new KeyValuePair(
                            "19", 
                            "Skole/institution", 
                            false, 
                            "19"
                        ),
                        new KeyValuePair(
                            "20", 
                            "Slot", 
                            false, 
                            "20"
                        ),
                        new KeyValuePair(
                            "21", 
                            "Stort hus", 
                            false, 
                            "21"
                        ),
                        new KeyValuePair(
                            "22", 
                            "Vandværk", 
                            false, 
                            "22"
                        )
                    }
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
                    true,
                    false,
                    "Tag billede af kort/tegning over placeringer, når alle målere er opsat *",
                    "<br>",
                    "ffe4e4",
                    6,
                    false,
                    0,
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
                
                dataItems.Add(new Text(
                        inc + 9,
                        false,
                        false,
                        $"Måler {i} - QR",
                        "",
                        "fff6df",
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
                // TODO seed this as a EntitySelect group 
                dataItems.Add(new SingleSelect(
                        inc + 10,
                        false,
                        false,
                        $"Måler {i} - Rumtype",
                        "",
                        "e8eaf6",
                        inc + 9,
                        false,
                        new List<KeyValuePair>
                        {
                            new KeyValuePair(
                                "1", 
                                "Soveværelse", 
                                false, 
                                "1"
                            ),
                            new KeyValuePair(
                                "2", 
                                "Stue", 
                                false, 
                                "2"
                            ),
                            new KeyValuePair(
                                "3", 
                                "Hobbyrum", 
                                false, 
                                "3"
                            ),
                            new KeyValuePair(
                                "4", 
                                "Gang", 
                                false, 
                                "4"
                            ),
                            new KeyValuePair(
                                "5", 
                                "Køkken", 
                                false, 
                                "5"
                            ),
                            new KeyValuePair(
                                "6", 
                                "Kontor", 
                                false, 
                                "6"
                            ),
                            new KeyValuePair(
                                "7", 
                                "Kælder", 
                                false, 
                                "7"
                            ),
                            new KeyValuePair(
                                "8", 
                                "Andre lukkede rum", 
                                false, 
                                "8"
                            )
                        }
                    )
                );
                dataItems.Add(new Number(
                        inc + 11,
                        false,
                        false,
                        $"Måler {i} - Etage",
                        "<u>Forklaring:<br></u>Vær opmærksom på at det er antallet af beboede etager, ikke det totale antal etager",
                        "fff6df",
                        inc + 10,
                        false,
                        "",
                        "",
                        0,
                        0,
                        ""
                    )
                );
                dataItems.Add(new Text(
                        inc + 12,
                        false,
                        false,
                        $"Måler {i} - Rumnavn",
                        "",
                        "fff6df",
                        inc + 11,
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
                dataItems.Add(new Picture(
                        inc + 13,
                        false,
                        false,
                        $"Måler {i} - Billede",
                        "<br>",
                        "ffe4e4",
                        inc + 12,
                        false,
                        0,
                        false
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
            var removalForm = new MainElement
                {
                    Id = 141709,
                    Repeated = 0,
                    Label = "(2) Radonmålinger Nedtagning",
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
                "https://eform.microting.com/app_files/uploads/20191008131612_14874_acb5333050e476e81c83bbcf5acd442c.pdf"
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
            dataItems.Add(new SaveButton(
                3,
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
                true,
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