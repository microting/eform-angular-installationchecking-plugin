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
        private static async Task<int> CreateCadastralTypeList(Core core)
        {
            EntityGroupList model = await core.Advanced_EntityGroupAll(
                "id", 
                "eform-angular-installationchecking-plugin-CadastralType",
                0, 1, Constants.FieldTypes.EntitySelect,
                false,
                Constants.WorkflowStates.NotRemoved);

            EntityGroup group;
            
            if (!model.EntityGroups.Any())
            {
                group = await core.EntityGroupCreate(Constants.FieldTypes.EntitySelect, 
                    "eform-angular-installationchecking-plugin-CadastralType");

                List<string> roomTypes = new List<string>()
                {
                    "Arbejdsplads", "Boligblok", "Boligbyggeri", "Butikker", "Delvist fritliggende hus", "Feriehus",
                    "Fritliggende hus", "Generelle faciliteter", "Hospital", "Hotel", "Hus i klippe",
                    "Information mangler", "Kaserne", "Kirke", "Kraftværk", "Plejehjem", "Rækkehus",
                    "Rækkehuse forbundet med garage", "Skole/institution", "Slot", "Stort hus", "Vandværk"
                };

                int i = 0;
                foreach (string roomType in roomTypes)
                {
                    await core.EntitySelectItemCreate(group.Id,roomType,i,i.ToString());
                }
            }
            else
            {
                group = model.EntityGroups.First();
            }

            return group.Id;
        }
        
        private static async Task<int> CreateRoomTypeList(Core core)
        {
            EntityGroupList model = await core.Advanced_EntityGroupAll(
                "id", 
                "eform-angular-installationchecking-plugin-RoomType",
                0, 1, Constants.FieldTypes.EntitySelect,
                false,
                Constants.WorkflowStates.NotRemoved);

            EntityGroup group;
            
            if (!model.EntityGroups.Any())
            {
                group = await core.EntityGroupCreate(Constants.FieldTypes.EntitySelect, 
                    "eform-angular-installationchecking-plugin-RoomType");
                
                List<string> roomTypes = new List<string>()
                {
                    "Soveværelse","Stue","Hobbyrum","Gang","Køkken","Kontor","Kælder","Andre lukkede rum"
                };

                int i = 0;
                foreach (string roomType in roomTypes)
                {
                    await core.EntitySelectItemCreate(group.Id,roomType,i,i.ToString());
                }
            }
            else
            {
                group = model.EntityGroups.First();
            }

            return group.Id;
        }

        public static async Task<int> CreateInstallationForm(Core core)
        {

            int roomTypeListId = await CreateRoomTypeList(core);
            int cadastralTypeId = await CreateCadastralTypeList(core);
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
            // TODO seed this as a EntitySelect group DONE
            dataItems.Add(new EntitySelect(1, 
                false, 
                false, 
                "Matrikeltype *", 
                "", 
                "e8eaf6",
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
                
                // TODO seed this as a EntitySelect group DONE
                dataItems.Add(new EntitySelect(
                    inc + 10, 
                    false, 
                    false, 
                    $"Måler {i} - Rumtype", 
                    "", 
                    "e8eaf6",
                    inc + 9, 
                    false, 0, 
                    roomTypeListId)
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