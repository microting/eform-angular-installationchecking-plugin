import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  InstallationModel,
  InstallationsAssignModel,
  InstallationsRequestModel,
} from '../models';
import { ApiBaseService } from 'src/app/common/services';
import {
  OperationDataResult,
  OperationResult,
  Paged,
} from 'src/app/common/models';

export let InstallationsMethods = {
  Index: 'api/installationchecking-pn/installations/index',
  Get: 'api/installationchecking-pn/installations',
  Create: 'api/installationchecking-pn/installations/create',
  Assign: 'api/installationchecking-pn/installations/assign',
  Retract: 'api/installationchecking-pn/installations/retract',
  Archive: 'api/installationchecking-pn/installations/archive',
  Excel: 'api/installationchecking-pn/installations/excel',
};
@Injectable()
export class InstallationsService {
  constructor(private apiBaseService: ApiBaseService) {}

  getList(
    request: InstallationsRequestModel
  ): Observable<OperationDataResult<Paged<InstallationModel>>> {
    return this.apiBaseService.post(InstallationsMethods.Index, request);
  }

  getSingle(id: number): Observable<OperationDataResult<InstallationModel>> {
    return this.apiBaseService.get(InstallationsMethods.Get + '/' + id);
  }

  create(customerId: number): Observable<OperationResult> {
    return this.apiBaseService.post(InstallationsMethods.Create, customerId);
  }

  assign(model: InstallationsAssignModel): Observable<OperationResult> {
    return this.apiBaseService.post(InstallationsMethods.Assign, model);
  }

  retract(id: number): Observable<OperationResult> {
    return this.apiBaseService.post(InstallationsMethods.Retract, id);
  }

  archive(id: number): Observable<OperationResult> {
    return this.apiBaseService.post(InstallationsMethods.Archive, id);
  }

  excel(id: number): Observable<any> {
    return this.apiBaseService.getBlobData(
      InstallationsMethods.Excel + '/' + id
    );
  }
}
