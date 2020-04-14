import Page from '../Page';
import customersPage from './Customers.page';

export class CustomersModalPage extends Page {
    constructor() {
        super();
    }

    public get createBtn() {
        $('#createCustomerBtn').waitForDisplayed({timeout: 20000});
        $('#createCustomerBtn').waitForClickable({timeout: 20000});
        return $('#createCustomerBtn');
    }

    public get cancelCreateBtn() {
        $('#cancelCreateCustomerBtn').waitForDisplayed({timeout: 20000});
        $('#cancelCreateCustomerBtn').waitForClickable({timeout: 20000});
        return $('#cancelCreateCustomerBtn');
    }

    public get saveEditBtn() {
        $('#saveEditBtn').waitForDisplayed({timeout: 20000});
        $('#saveEditBtn').waitForClickable({timeout: 20000});
        return $('#saveEditBtn');
    }

    public get cancelEditBtn() {
        $('#cancelEditBtn').waitForDisplayed({timeout: 20000});
        $('#cancelEditBtn').waitForClickable({timeout: 20000});
        return $('#cancelEditBtn');
    }

    public get saveDeleteBtn() {
        $('#customerSaveDeleteBtn').waitForDisplayed({timeout: 20000});
        $('#customerSaveDeleteBtn').waitForClickable({timeout: 20000});
        return $('#customerSaveDeleteBtn');
    }

    public get cancelDeleteBtn() {
        $('#customerDeleteCancelBtn').waitForDisplayed({timeout: 20000});
        $('#customerDeleteCancelBtn').waitForClickable({timeout: 20000});
        return $('#customerDeleteCancelBtn');
    }

    public get createCreatedByInput() {
        $('#createCreatedBy').waitForDisplayed({timeout: 20000});
        $('#createCreatedBy').waitForClickable({timeout: 20000});
        return $('#createCreatedBy');
    }

    public get editCreatedByInput() {
        $('#editCreatedBy').waitForDisplayed({timeout: 20000});
        $('#editCreatedBy').waitForClickable({timeout: 20000});
        return $('#editCreatedBy');
    }

    public get createCustomerNo() {
        $('#createCustomerNo').waitForDisplayed({timeout: 20000});
        $('#createCustomerNo').waitForClickable({timeout: 20000});
        return $('#createCustomerNo');
    }

    public get editCustomerNo() {
        $('#editCustomerNo').waitForDisplayed({timeout: 20000});
        $('#editCustomerNo').waitForClickable({timeout: 20000});
        return $('#editCustomerNo');
    }

    public get createContactPerson() {
        $('#createContactPerson').waitForDisplayed({timeout: 20000});
        $('#createContactPerson').waitForClickable({timeout: 20000});
        return $('#createContactPerson');
    }

    public get editContactPerson() {
        $('#editContactPerson').waitForDisplayed({timeout: 20000});
        $('#editContactPerson').waitForClickable({timeout: 20000});
        return $('#editContactPerson');
    }

    public get createCompanyName() {
        $('#createCompanyName').waitForDisplayed({timeout: 20000});
        $('#createCompanyName').waitForClickable({timeout: 20000});
        return $('#createCompanyName');
    }

    public get editCompanyName() {
        $('#editCompanyName').waitForDisplayed({timeout: 20000});
        $('#editCompanyName').waitForClickable({timeout: 20000});
        return $('#editCompanyName');
    }

    public get createCompanyAddress() {
        $('#createCompanyAddress').waitForDisplayed({timeout: 20000});
        $('#createCompanyAddress').waitForClickable({timeout: 20000});
        return $('#createCompanyAddress');
    }

    public get editCompanyAddress() {
        $('#editCompanyAddress').waitForDisplayed({timeout: 20000});
        $('#editCompanyAddress').waitForClickable({timeout: 20000});
        return $('#editCompanyAddress');
    }

    public get createZipCode() {
        $('#createZipCode').waitForDisplayed({timeout: 20000});
        $('#createZipCode').waitForClickable({timeout: 20000});
        return $('#createZipCode');
    }

    public get editZipCode() {
        $('#editZipCode').waitForDisplayed({timeout: 20000});
        $('#editZipCode').waitForClickable({timeout: 20000});
        return $('#editZipCode');
    }

    public get createCityName() {
        $('#createCityName').waitForDisplayed({timeout: 20000});
        $('#createCityName').waitForClickable({timeout: 20000});
        return $('#createCityName');
    }

    public get editCityName() {
        $('#editCityName').waitForDisplayed({timeout: 20000});
        $('#editCityName').waitForClickable({timeout: 20000});
        return $('#editCityName');
    }

    public get createPhone() {
        $('#createPhone').waitForDisplayed({timeout: 20000});
        $('#createPhone').waitForClickable({timeout: 20000});
        return $('#createPhone');
    }

    public get editPhone() {
        $('#editPhone').waitForDisplayed({timeout: 20000});
        $('#editPhone').waitForClickable({timeout: 20000});
        return $('#editPhone');
    }

    public get createEmail() {
        $('#createEmail').waitForDisplayed({timeout: 20000});
        $('#createEmail').waitForClickable({timeout: 20000});
        return $('#createEmail');
    }

    public get editEmail() {
        $('#editEmail').waitForDisplayed({timeout: 20000});
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
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
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
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }

    public createEmptyCustomer() {
        this.createBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }

    public deleteCustomer() {
        this.saveDeleteBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }
}

const customersModalPage = new CustomersModalPage();
export default customersModalPage;
