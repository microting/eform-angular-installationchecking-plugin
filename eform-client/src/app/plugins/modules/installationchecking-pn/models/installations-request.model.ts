import { InstallationStateEnum, InstallationTypeEnum } from '../const/enums';

export class InstallationsRequestModel {
  nameFilter: string;
  type: InstallationTypeEnum;
  state: InstallationStateEnum;

  sort: string;
  offset: number;
  pageSize: number;
  isSortDsc: boolean;

  constructor() {
    this.sort = 'Id';
    this.isSortDsc = true;
    this.pageSize = 10;
    this.offset = 0;
  }
}
