import {InstallationStateEnum, InstallationTypeEnum} from '../const/enums';

export class InstallationModel {
  id: number;

  companyName: string;
  companyAddress: string;
  companyAddress2: string;
  zipCode: string;
  cityName: string;
  countryCode: string;

  type: InstallationTypeEnum;
  state: InstallationStateEnum;

  dateInstall: Date;
  dateRemove: Date;
  dateActRemove: Date;

  employeeId: number;
  customerId: number;
  sdkCaseId: number;
}
