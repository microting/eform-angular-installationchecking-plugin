import Page from '../Page';
import {expect} from 'chai';
import {PageWithNavbarPage} from '../PageWithNavbar.page';
import customersModalPage from './CustomersModal.page';
import customersSettingsPage from './CustomerSettings.page';

export class CustomersPage extends PageWithNavbarPage {
    constructor() {
        super();
    }

    public configureSearchableList(listName: string) {
        customersPage.Navbar.advancedDropdown();
        $('#spinner-animation').waitForDisplayed(90000, true);
        customersPage.Navbar.clickonSubMenuItem('Søgbar Lister');
        $('#spinner-animation').waitForDisplayed(90000, true);
        const newSearchListBtn = browser.$('#createEntitySearchBtn');
        const numberOfListsBefore = browser.$$('#tableBody > tr').length;
        newSearchListBtn.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
        const fieldElement = browser.$('#createName');
        fieldElement.addValue(listName);
        const confirmBtn = browser.$('#entitySearchCreateSaveBtn');
        confirmBtn.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
        const numberOfListsAfter = browser.$$('#tableBody > tr').length;
        expect(numberOfListsAfter, 'Number of rows is less than expected').equal(numberOfListsBefore + 1);

        // Configure List
        const nameOfList = 'My testing list';
        customersPage.goToCustomersPage();
        $('#spinner-animation').waitForDisplayed(90000, true);
        customersPage.settingsCustomerBtn.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
        const searchField = customersSettingsPage.getSearchField();
        searchField.addValue(nameOfList);
        const listChoices = customersSettingsPage.getListOfChoices();
        const choice = listChoices[0];
        $('#spinner-animation').waitForDisplayed(90000, true);
        choice.click();
        const fieldToCheck = customersSettingsPage.selectedListField();
        expect(fieldToCheck.getText(), 'Searchable list is not selected').equal('My testing list');
        customersSettingsPage.saveSettings();
    }

    public createCustomer(companyName: string) {
        customersPage.goToCustomersPage();
        customersPage.newCustomerBtn.click();
        $('#spinner-animation').waitForDisplayed(10000, true);
        $('#spinner-animation').waitForDisplayed(90000, true);
        const customerObject = {
            createdBy: 'John Smith',
            customerNo: '1',
            contactPerson: 'Samantha Black',
            companyName: companyName,
            companyAddress: 'Test',
            zipCode: '021551',
            cityName: 'Odense',
            phone: '123124',
            email: 'user@user.com'
        };
        const rowCountBeforeCreation = customersPage.rowNum();
        $('#spinner-animation').waitForDisplayed(90000, true);
        customersModalPage.createCustomer(customerObject);
        $('#spinner-animation').waitForDisplayed(10000, true);
        const rowCountAfterCreation = customersPage.rowNum();
        $('#spinner-animation').waitForDisplayed(90000, true);
        expect(rowCountAfterCreation, 'Number of rows hasn\'t changed after creating new user').equal(rowCountBeforeCreation + 1);
        const lastCustomer: CustomersRowObject = customersPage.getCustomer(customersPage.rowNum());
        expect(lastCustomer.createdBy, 'Created by of created customer is incorrect').equal(customerObject.createdBy);
        expect(lastCustomer.customerNo, 'Customer number of created customer is incorrect').equal(customerObject.customerNo);
        expect(lastCustomer.contactPerson, 'Contact person of created customer is incorrect').equal(customerObject.contactPerson);
        expect(lastCustomer.companyName, 'Company name of created customer is incorrect').equal(customerObject.companyName);
        expect(lastCustomer.companyAddress, 'Company address of created customer is incorrect').equal(customerObject.companyAddress);
        expect(lastCustomer.zipCode, 'Zip code of created customer is incorrect').equal(customerObject.zipCode);
        expect(lastCustomer.cityName, 'City name of created customer is incorrect').equal(customerObject.cityName);
        expect(lastCustomer.phone, 'Phone of created customer is incorrect').equal(customerObject.phone);
        expect(lastCustomer.email, 'Email of created customer is incorrect').equal(customerObject.email);
        $('#spinner-animation').waitForDisplayed(90000, true);
    }

    public getListOfChoices() {
        return browser.$$('.ng-option');
    }
    public  selectedListField() {
        return browser.$('.ng-value .ng-value-label');
    }

    public rowNum(): number {
        browser.pause(500);
        return browser.$$('#mainTableBody > tr').length;
    }
    public clickIdSort() {
        browser.$('#IdTableHeader').click();
        $('#spinner-animation').waitForDisplayed(90000, true);
    }
    public clickContactSort() {
        browser.$('#ContactPersonTableHeader').click();
        $('#spinner-animation').waitForDisplayed(90000, true);
    }

    public clickCompanySort() {
        browser.$('#CompanyNameTableHeader').click();
        $('#spinner-animation').waitForDisplayed(90000, true);
    }

    public getCustomerValue(selector: any, row: number) {
        if (selector === 'Id') {
            return  parseInt( $('#mainTableBody').$(`tr:nth-child(${row})`).$('#' + selector).getText(), 10);
        } else {
            return $('#mainTableBody').$(`tr:nth-child(${row})`).$('#' + selector).getText();
        }
    }

    getCustomer(num): CustomersRowObject {
        browser.pause(500);
        return new CustomersRowObject(num);
    }

    public get newCustomerBtn() {
        $('#createCustomerBtn').waitForDisplayed(50000);
        $('#newCustomerBtn').waitForDisplayed(20000);
        $('#newCustomerBtn').waitForClickable({timeout: 20000});
        return $('#newCustomerBtn');
    }

    public get customersSettingsBtn() {
        $('#firstName').waitForDisplayed(20000);
        $('#firstName').waitForClickable({timeout: 20000});
        return $('#firstName');
    }

    public get importCustomersSettingsBtn() {
        $('#lastName').waitForDisplayed(20000);
        $('#lastName').waitForClickable({timeout: 20000});
        return $('#lastName');
    }

    // same purpose as previous method?
    public  importCustomerBtn() {
        $('#importCustomer').waitForDisplayed(20000);
        $('#importCustomer').waitForClickable({timeout: 20000});
        return $('#importCustomer');
    }

    public  goToImportBtn() {
        this.importCustomerBtn().click();
        $('#spinner-animation').waitForDisplayed(90000, true);
    }

    public get saveImportCustomersBtn() {
        $('#saveCreateBtn').waitForDisplayed(20000);
        $('#saveCreateBtn').waitForClickable({timeout: 20000});
        return $('#saveCreateBtn');
    }

    public get cancelImportCustomersBtn() {
        $('#saveCreateBtn').waitForDisplayed(20000);
        $('#saveCreateBtn').waitForClickable({timeout: 20000});
        return $('#saveCreateBtn');
    }

    public get deleteCustomerBtn() {
        $('#cancelCreateBtn').waitForDisplayed(20000);
        $('#cancelCreateBtn').waitForClickable({timeout: 20000});
        return $('#cancelCreateBtn');
    }

    public get editCustomerBtn() {
        $('#editCustomerBtn').waitForDisplayed(20000);
        $('#editCustomerBtn').waitForClickable({timeout: 20000});
        return $('#editCustomerBtn');
    }

    public get customersButton() {
        $('#customers-pn').waitForDisplayed(20000);
        $('#customers-pn').waitForClickable({timeout: 20000});
        return $('#customers-pn');
    }

    public get settingsCustomerBtn() {
        return browser.$('#settingsCustomerBtn');
    }

    public goToCustomerSettings() {
        const elem = browser.$('button .btn .btn-danger');
        elem.click();
        $('#spinner-animation').waitForDisplayed(90000, true);
    }

    public goToCustomersPage() {
        this.customersButton.click();
        browser.pause(20000);
    }

    public get saveDeleteBtn() {
        $('#customerSaveDeleteBtn').waitForDisplayed(20000);
        $('#customerSaveDeleteBtn').waitForClickable({timeout: 20000});
        return $('#customerSaveDeleteBtn');
    }
}

const customersPage = new CustomersPage();
export default customersPage;

export class CustomersRowObject {
    constructor(rowNumber) {
        console.log('rowNumber is ' + rowNumber);
        this.createdBy = $$('#CreatedBy_' + (rowNumber - 1))[0].getText();
        this.customerNo = $$('#CustomerNo_' + (rowNumber - 1))[0].getText();
        this.contactPerson = $$('#ContactPerson_' + (rowNumber - 1))[0].getText();
        this.companyName = $$('#CompanyName_' + (rowNumber - 1))[0].getText();
        this.companyAddress = $$('#CompanyAddress_' + (rowNumber - 1))[0].getText();
        this.zipCode = $$('#ZipCode_' + (rowNumber - 1))[0].getText();
        this.cityName = $$('#CityName_' + (rowNumber - 1))[0].getText();
        this.email = $$('#Email_' + (rowNumber - 1))[0].getText();
        this.phone = $$('#Phone_' + (rowNumber - 1))[0].getText();
        this.editBtn = $$('#editCustomerBtn_' + (rowNumber - 1))[0];
        this.copyBtn = $$('#copyCustomerBtn_' + (rowNumber - 1))[0];
        this.deleteBtn = $$('#deleteCustomerBtn_' + (rowNumber - 1))[0];
    }

    public id;
    public version;
    public updatedByUserId;
    public createdBy;
    public customerNo;
    public contactPerson;
    public companyName;
    public companyAddress;
    public zipCode;
    public cityName;
    public email;
    public phone;
    public editBtn;
    public copyBtn;
    public deleteBtn;
}
