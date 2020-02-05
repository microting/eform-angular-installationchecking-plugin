import {InstallationStateEnum, InstallationTypeEnum} from '../const/enums';

export class InstallationModel {
  id: number;

  companyName: string;
  companyAddress: string;
  companyAddress2: string;
  zipCode: string;
  cityName: string;
  countryCode: string;

  dateInstall: Date;
  dateRemove: Date;
  dateActRemove: Date;

  assignedTo: string;

  type: InstallationTypeEnum;
  state: InstallationStateEnum;

  installationEmployeeId: number;
  removalEmployeeId: number;
  customerId: number;
  installationSdkCaseId: number;
  removalSdkCaseId: number;
  installationSdkCaseDbId: number;
  removalSdkCaseDbId: number;
  removalFormId: number;
  installationFormId: number;

  assign = false;
}
