import {Injectable} from '@angular/core';
import {BaseService} from '../../../../common/services/base.service';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {Observable} from 'rxjs';
import {OperationDataResult, OperationResult} from '../../../../common/models';
import {InstallationCheckingBaseSettingsModel} from '../models';

export let InstallationCheckingSettingsMethods = {
  InstallationCheckingSettings: 'api/installationchecking-pn/settings'
};
@Injectable()
export class InstallationCheckingPnSettingsService extends BaseService {
  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  getAllSettings(): Observable<OperationDataResult<any>> {
    return this.get(InstallationCheckingSettingsMethods.InstallationCheckingSettings);
  }

  updateSettings(model: InstallationCheckingBaseSettingsModel): Observable<OperationResult> {
    return this.post(InstallationCheckingSettingsMethods.InstallationCheckingSettings, model);
  }
}
