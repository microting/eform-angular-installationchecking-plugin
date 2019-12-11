import Page from '../Page';

export class InstallationsPage extends Page {
  constructor() {
    super();
  }
  public get rowNum(): number {
    return $$('#tableBody > tr').length;
  }
  public InstallationCheckingDropDown() {
    browser.element(`//*[contains(@class, 'dropdown')]//*[contains(text(), 'Planlægning')]`).click();
  }
  public get InstallationsBtn() {
    return browser.element('#installationchecking-pn-installation');
  }
  goToInstallationsPage() {
    this.InstallationCheckingDropDown();
    browser.pause(1000);
    this.InstallationsBtn.click();
    browser.pause(8000);
  }
}

const installationsPage = new InstallationsPage();
export default installationsPage;

export class InstallationRowObject {
  constructor(rowNumber) {
    this.id = $$('#idTableHeader')[rowNumber - 1].getText();
    this.companyName = $$('#companyNameTableHeader')[rowNumber - 1].getText();
    this.companyAddress = $$('#companyAddressTableHeader')[rowNumber - 1].getText();
    this.companyAddress2 = $$('#companyAddress2TableHeader')[rowNumber - 1].getText();
    this.zipCode = $$('#zipCodeTableHeader')[rowNumber - 1].getText();
    this.cityName = $$('#cityNameTableHeader')[rowNumber - 1].getText();
    this.countryCode = $$('#countryCodeTableHeader')[rowNumber - 1].getText();
    this.dateInstall = $$('#dateInstallTableHeader')[rowNumber - 1].getText();
    this.assignedTo = $$('#assignedToTableHeader')[rowNumber - 1].getText();
  }
  public id;
  public version;
  public date;
  public companyName;
  public companyAddress;
  public companyAddress2;
  public zipCode;
  public cityName;
  public time;
  public countryCode;
  public dateInstall;
  public assignedTo;
  public name;
  public status;
}
