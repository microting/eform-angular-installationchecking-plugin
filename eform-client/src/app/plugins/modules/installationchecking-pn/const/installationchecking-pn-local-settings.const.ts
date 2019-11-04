import {
  ApplicationPageModel,
  PageSettingsModel
} from 'src/app/common/models/settings/application-page-settings.model';

export const InstallationCheckingPnLocalSettings =
  new ApplicationPageModel({
      name: 'InstallationCheckingPn',
      settings: new PageSettingsModel({
        pageSize: 10,
        sort: '',
        isSortDsc: false
      })
    }
  );
