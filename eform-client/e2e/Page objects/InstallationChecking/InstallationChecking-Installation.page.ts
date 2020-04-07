import Page from '../Page';

export class InstallationCheckingInstallationPage extends Page {
  constructor() {
    super();
  }
  public get rowNum(): number {
    return $$('#tableBody > tr').length;
  }
  public installationCheckingDropDown() {
    $(`//*[contains(@class, 'dropdown')]//*[contains(text(), 'Planlægning')]`).click();
  }
  public get installationBtn() {
    $('#installationchecking-pn-installation').waitForDisplayed(20000);
    $('#installationchecking-pn-installation').waitForClickable({timeout: 20000});
    return $('#installationchecking-pn-installation');
  }
  public get installationCreateBtn() {
    $('#createInstallationBtn').waitForDisplayed(20000);
    $('#createInstallationBtn').waitForClickable({timeout: 20000});
    return $('#createInstallationBtn');
  }
  public get installationAssignBtn() {
    $('#installationAssignBtn').waitForDisplayed(20000);
    $('#installationAssignBtn').waitForClickable({timeout: 20000});
    return $('#installationAssignBtn');
  }
  public get installationAssignBtnSave() {
    $('#installationAssignBtnSave').waitForDisplayed(20000);
    $('#installationAssignBtnSave').waitForClickable({timeout: 20000});
    return $('#installationAssignBtnSave');
  }
  public get installationAssignBtnSaveCancel() {
    $('#installationAssignBtnSaveCancel').waitForDisplayed(20000);
    $('#installationAssignBtnSaveCancel').waitForClickable({timeout: 20000});
    return $('#installationAssignBtnSaveCancel');
  }
  public get installationRetractBtn() {
    $('#installationRetractBtn').waitForDisplayed(20000);
    $('#installationRetractBtn').waitForClickable({timeout: 20000});
    return $('#installationRetractBtn');
  }
  public get installationRetractSaveBtn() {
    $('#installationRetractSaveBtn').waitForDisplayed(20000);
    $('#installationRetractSaveBtn').waitForClickable({timeout: 20000});
    return $('#installationRetractSaveBtn');
  }
  public get installationRetractSaveCancelBtn() {
    $('#installationRetractSaveCancelBtn').waitForDisplayed(20000);
    $('#installationRetractSaveCancelBtn').waitForClickable({timeout: 20000});
    return $('#installationRetractSaveCancelBtn');
  }
  public get installationCreateNameBox() {
    $('#createInstallationName').waitForDisplayed(20000);
    $('#createInstallationName').waitForClickable({timeout: 20000});
    return $('#createInstallationName');
  }
  public get installationAssignCheckbox() {
    $('#assignCheckbox').waitForDisplayed(20000);
    $('#assignCheckbox').waitForClickable({timeout: 20000});
    return $('#assignCheckbox');
  }
  public get installationCreateSiteCheckbox() {
    $('#checkbox').waitForDisplayed(20000);
    $('#checkbox').waitForClickable({timeout: 20000});
    return $('#checkbox');
  }
  public get installationCreateSaveBtn() {
    $('#installationCreateSaveBtn').waitForDisplayed(20000);
    $('#installationCreateSaveBtn').waitForClickable({timeout: 20000});
    return $('#installationCreateSaveBtn');
  }
  public get installationCreateCancelBtn() {
    $('#installationCreateCancelBtn').waitForDisplayed(20000);
    $('#installationCreateCancelBtn').waitForClickable({timeout: 20000});
    return $('#installationCreateCancelBtn');
  }
  public get installationUpdateNameBox() {
    $('#updateInstallationName').waitForDisplayed(20000);
    $('#updateInstallationName').waitForClickable({timeout: 20000});
    return $('#updateInstallationName');
  }
  public get installationUpdateSiteCheckbox() {
    $('#checkbox').waitForDisplayed(20000);
    $('#checkbox').waitForClickable({timeout: 20000});
    return $('#checkbox');
  }
  public get installationUpdateSaveBtn() {
    $('#installationUpdateSaveBtn').waitForDisplayed(20000);
    $('#installationUpdateSaveBtn').waitForClickable({timeout: 20000});
    return $('#installationUpdateSaveBtn');
  }
  public get installationUpdateCancelBtn() {
    $('#installationUpdateCancelBtn').waitForDisplayed(20000);
    $('#installationUpdateCancelBtn').waitForClickable({timeout: 20000});
    return $('#installationUpdateCancelBtn');
  }
  public get installationDeleteId() {
    $('#selectedInstallationId').waitForDisplayed(20000);
    $('#selectedInstallationId').waitForClickable({timeout: 20000});
    return $('#selectedInstallationId');
  }
  public get installationDeleteName() {
    $('#selectedInstallationName').waitForDisplayed(20000);
    $('#selectedInstallationName').waitForClickable({timeout: 20000});
    return $('#selectedInstallationName');
  }
  public get installationDeleteDeleteBtn() {
    $('#installationDeleteDeleteBtn').waitForDisplayed(20000);
    $('#installationDeleteDeleteBtn').waitForClickable({timeout: 20000});
    return $('#installationDeleteDeleteBtn');
  }
  public get installationDeleteCancelBtn() {
    $('#installationDeleteCancelBtn').waitForDisplayed(20000);
    $('#installationDeleteCancelBtn').waitForClickable({timeout: 20000});
    return $('#installationDeleteCancelBtn');
  }
  public get page2Object() {
    return $(`//*[div]//*[contains(@class, 'd-flex justify-content-center')]//*[ul]//*[contains(@class, 'page-item')]//*[contains(text(), '2')]`);
  }
  goToInstallationsPage() {
    this.installationCheckingDropDown();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationBtn.click();
  }

  createInstallation(name: string) {
    this.installationCreateBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    const searchField = installationPage.getCustomerSearchField();
    searchField.addValue(name);
    browser.pause(500);
    const listChoices = installationPage.getCustomerListOfChoices();
    const choice = listChoices[0];
    $('#spinner-animation').waitForDisplayed(90000, true);
    choice.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationCreateSaveBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }
  createInstallation_Cancels() {
    this.installationCreateBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationCreateCancelBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }
  retractInstallation() {
    const installation = installationPage.getFirstRowObject();
    installation.retractBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationRetractSaveBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }

  retractInstallation_Cancels() {
    const installation = installationPage.getFirstRowObject();
    installation.retractBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationRetractSaveCancelBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }

  public  getCustomerSearchField() {
    $('#selectCustomer .ng-input > input').waitForDisplayed(20000);
    $('#selectCustomer .ng-input > input').waitForClickable({timeout: 20000});
    return $('#selectCustomer .ng-input > input');
  }
  public getCustomerListOfChoices() {
    return $$('#selectCustomer .ng-option');
  }
  public  selectedListField() {
    return $('#selectCustomer .ng-value .ng-value-label');
  }

  public  getDeviceUserSearchField() {
    $('#selectDeviceUser .ng-input > input').waitForDisplayed(20000);
    $('#selectDeviceUser .ng-input > input').waitForClickable({timeout: 20000});
    return $('#selectDeviceUser .ng-input > input');
  }
  public getDeviceUserListOfChoices() {
    return $$('#selectDeviceUser .ng-option');
  }

  assignInstallation(deviceUserName: string) {
    const installation = installationPage.getFirstRowObject();
    installation.assignCheckbox.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationAssignBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    const searchField = installationPage.getDeviceUserSearchField();
    searchField.addValue(deviceUserName);
    const listChoices = installationPage.getDeviceUserListOfChoices();
    const choice = listChoices[0];
    $('#spinner-animation').waitForDisplayed(90000, true);
    choice.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationAssignBtnSave.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }

  assignInstallation_Cancels() {
    const installation = installationPage.getFirstRowObject();
    installation.assignCheckbox.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationAssignBtn.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    this.installationAssignBtnSaveCancel.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
    installation.assignCheckbox.click();
    $('#spinner-animation').waitForDisplayed(90000, true);
  }

  getFirstRowObject(): InstallationPageRowObject {
    browser.pause(500);
    return new InstallationPageRowObject(1);
  }
  getInstallation(num): InstallationPageRowObject {
    browser.pause(500);
    return new InstallationPageRowObject(num);
  }
}

const installationPage = new InstallationCheckingInstallationPage();
export default installationPage;

export class InstallationPageRowObject {
  constructor(rowNum) {
    if ($$('#installationId')[rowNum - 1]) {
      this.id = $$('#installationId')[rowNum - 1];
      try {
        this.companyName = $$('#installationCompanyName')[rowNum - 1].getText();
        // this.companyAddress = $$('#companyAddressTableHeader')[rowNum - 1].getText();
        // this.companyAddress2 = $$('#companyAddress2TableHeader')[rowNum - 1].getText();
        // this.zipCode = $$('#zipCodeTableHeader')[rowNum - 1].getText();
        // this.cityName = $$('#cityNameTableHeader')[rowNum - 1].getText();
        // this.countryCode = $$('#countryCodeTableHeader')[rowNum - 1].getText();
        // this.dateInstall = $$('#dateInstallTableHeader')[rowNum - 1].getText();
        this.assignedTo = $$('#installationAssignedTo')[rowNum - 1].getText();
      } catch (e) {}
      this.assignCheckbox = $$(`#assignCheckbox_${rowNum - 1}`)[rowNum - 1];
      this.retractBtn = $$('#installationRetractBtn')[rowNum - 1];
    }
  }

  public id;
  public companyName;
  public assignCheckbox;
  public retractBtn;
  public version;
  public date;
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
