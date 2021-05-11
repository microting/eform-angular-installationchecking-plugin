import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  OperationDataResult,
  OperationResult,
} from '../../../../common/models';
import { InstallationCheckingBaseSettingsModel } from '../models';
import { ApiBaseService } from 'src/app/common/services';

export let InstallationCheckingSettingsMethods = {
  InstallationCheckingSettings: 'api/installationchecking-pn/settings',
};

@Injectable()
export class InstallationCheckingPnSettingsService {
  constructor(private apiBaseService: ApiBaseService) {}

  getAllSettings(): Observable<OperationDataResult<any>> {
    return this.apiBaseService.get(
      InstallationCheckingSettingsMethods.InstallationCheckingSettings
    );
  }

  updateSettings(
    model: InstallationCheckingBaseSettingsModel
  ): Observable<OperationResult> {
    return this.apiBaseService.post(
      InstallationCheckingSettingsMethods.InstallationCheckingSettings,
      model
    );
  }
}
