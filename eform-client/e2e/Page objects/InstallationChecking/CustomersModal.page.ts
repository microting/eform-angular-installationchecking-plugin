import Page from '../Page';
import customersPage from './Customers.page';

export class CustomersModalPage extends Page {
  constructor() {
    super();
  }

  public get createBtn() {
    $('#createCustomerBtn').waitForDisplayed(20000);
$('#createCustomerBtn').waitForClickable({timeout: 20000});
return $('#createCustomerBtn');
  }

  public get cancelCreateBtn() {
    $('#cancelCreateCustomerBtn').waitForDisplayed(20000);
$('#cancelCreateCustomerBtn').waitForClickable({timeout: 20000});
return $('#cancelCreateCustomerBtn');
  }

  public get saveEditBtn() {
    $('#saveEditBtn').waitForDisplayed(20000);
$('#saveEditBtn').waitForClickable({timeout: 20000});
return $('#saveEditBtn');
  }

  public get cancelEditBtn() {
    $('#cancelEditBtn').waitForDisplayed(20000);
$('#cancelEditBtn').waitForClickable({timeout: 20000});
return $('#cancelEditBtn');
  }

  public get saveDeleteBtn() {
    $('#customerSaveDeleteBtn').waitForDisplayed(20000);
$('#customerSaveDeleteBtn').waitForClickable({timeout: 20000});
return $('#customerSaveDeleteBtn');
  }

  public get cancelDeleteBtn() {
    $('#customerDeleteCancelBtn').waitForDisplayed(20000);
$('#customerDeleteCancelBtn').waitForClickable({timeout: 20000});
return $('#customerDeleteCancelBtn');
  }

  public get createCreatedByInput() {
    $('#createCreatedBy').waitForDisplayed(20000);
$('#createCreatedBy').waitForClickable({timeout: 20000});
return $('#createCreatedBy');
  }

  public get editCreatedByInput() {
    $('#editCreatedBy').waitForDisplayed(20000);
$('#editCreatedBy').waitForClickable({timeout: 20000});
return $('#editCreatedBy');
  }

  public get createCustomerNo() {
    $('#createCustomerNo').waitForDisplayed(20000);
$('#createCustomerNo').waitForClickable({timeout: 20000});
return $('#createCustomerNo');
  }

  public get editCustomerNo() {
    $('#editCustomerNo').waitForDisplayed(20000);
$('#editCustomerNo').waitForClickable({timeout: 20000});
return $('#editCustomerNo');
  }

  public get createContactPerson() {
    $('#createContactPerson').waitForDisplayed(20000);
$('#createContactPerson').waitForClickable({timeout: 20000});
return $('#createContactPerson');
  }

  public get editContactPerson() {
    $('#editContactPerson').waitForDisplayed(20000);
$('#editContactPerson').waitForClickable({timeout: 20000});
return $('#editContactPerson');
  }

  public get createCompanyName() {
    $('#createCompanyName').waitForDisplayed(20000);
$('#createCompanyName').waitForClickable({timeout: 20000});
return $('#createCompanyName');
  }

  public get editCompanyName() {
    $('#editCompanyName').waitForDisplayed(20000);
$('#editCompanyName').waitForClickable({timeout: 20000});
return $('#editCompanyName');
  }

  public get createCompanyAddress() {
    $('#createCompanyAddress').waitForDisplayed(20000);
$('#createCompanyAddress').waitForClickable({timeout: 20000});
return $('#createCompanyAddress');
  }

  public get editCompanyAddress() {
    $('#editCompanyAddress').waitForDisplayed(20000);
$('#editCompanyAddress').waitForClickable({timeout: 20000});
return $('#editCompanyAddress');
  }

  public get createZipCode() {
    $('#createZipCode').waitForDisplayed(20000);
$('#createZipCode').waitForClickable({timeout: 20000});
return $('#createZipCode');
  }

  public get editZipCode() {
    $('#editZipCode').waitForDisplayed(20000);
$('#editZipCode').waitForClickable({timeout: 20000});
return $('#editZipCode');
  }

  public get createCityName() {
    $('#createCityName').waitForDisplayed(20000);
$('#createCityName').waitForClickable({timeout: 20000});
return $('#createCityName');
  }

  public get editCityName() {
    $('#editCityName').waitForDisplayed(20000);
$('#editCityName').waitForClickable({timeout: 20000});
return $('#editCityName');
  }

  public get createPhone() {
    $('#createPhone').waitForDisplayed(20000);
$('#createPhone').waitForClickable({timeout: 20000});
return $('#createPhone');
  }

  public get editPhone() {
    $('#editPhone').waitForDisplayed(20000);
$('#editPhone').waitForClickable({timeout: 20000});
return $('#editPhone');
  }

  public get createEmail() {
    $('#createEmail').waitForDisplayed(20000);
$('#createEmail').waitForClickable({timeout: 20000});
return $('#createEmail');
  }

  public get editEmail() {
    $('#editEmail').waitForDisplayed(20000);
$('#editEmail').waitForClickable({timeout: 20000});
return $('#editEmail');
  }

  public createCustomer(data: any) {
    this.createCreatedByInput.setValue(data.createdBy);
    this.createCustomerNo.setValue(data.customerNo);
    this.createContactPerson.setValue(data.contactPerson);
    this.createCompanyName.setValue(data.companyName);
    this.createCompanyAddress.setValue(data.companyAddress);
    this.createZipCode.setValue(data.zipCode);
    this.createCityName.setValue(data.cityName);
    this.createPhone.setValue(data.phone);
    this.createEmail.setValue(data.email);
    this.createBtn.click();
    browser.pause(16000);
  }

  public updateCustomer(data: any) {
    this.editCreatedByInput.setValue(data.createdBy);
    this.editCustomerNo.setValue(data.customerNo);
    this.editContactPerson.setValue(data.contactPerson);
    this.editCompanyName.setValue(data.companyName);
    this.editCompanyAddress.setValue(data.companyAddress);
    this.editZipCode.setValue(data.zipCode);
    this.editCityName.setValue(data.cityName);
    this.editPhone.setValue(data.phone);
    this.editEmail.setValue(data.email);
    this.saveEditBtn.click();
    browser.pause(16000);
  }

  public createEmptyCustomer() {
    this.createBtn.click();
    browser.pause(16000);
  }

  public deleteCustomer() {
    this.saveDeleteBtn.click();
    browser.pause(16000);
  }

}

const customersModalPage = new CustomersModalPage();
export default customersModalPage;
