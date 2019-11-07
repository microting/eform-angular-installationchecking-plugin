import {
  ApplicationPageModel,
  PageSettingsModel
} from 'src/app/common/models/settings/application-page-settings.model';

export const InstallationCheckingPnLocalSettings = [
  new ApplicationPageModel({
      name: 'Installations',
      settings: new PageSettingsModel({
        pageSize: 10,
        sort: '',
        isSortDsc: false
      })
    }
  ),
  new ApplicationPageModel({
      name: 'Removals',
      settings: new PageSettingsModel({
        pageSize: 10,
        sort: '',
        isSortDsc: false
      })
    }
  )
];
