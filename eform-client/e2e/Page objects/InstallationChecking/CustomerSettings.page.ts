import Page from '../Page';

export class CustomersSettingsPage extends Page {
    constructor() {
        super();
    }

    public get deleteCustomerBtn() {
        $('#cancelCreateBtn').waitForDisplayed({timeout: 20000});
        $('#cancelCreateBtn').waitForClickable({timeout: 20000});
        return $('#cancelCreateBtn');
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
        $('#saveDeleteBtn').waitForDisplayed({timeout: 20000});
        $('#saveDeleteBtn').waitForClickable({timeout: 20000});
        return $('#saveDeleteBtn');
    }

    public get cancelDeleteBtn() {
        $('#cancelDeleteBtn').waitForDisplayed({timeout: 20000});
        $('#cancelDeleteBtn').waitForClickable({timeout: 20000});
        return $('#cancelDeleteBtn');
    }
    public getCheckboxById(id: string) {
        return $('#checkbox' + id);
    }

    public clickCheckboxById(id: string) {
        const el = $('#mat-checkbox' + id);
        el.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }
    public  getSearchField() {
        $('.ng-input > input').waitForDisplayed({timeout: 20000});
        $('.ng-input > input').waitForClickable({timeout: 20000});
        return $('.ng-input > input');
    }
    public getListOfChoices() {
        return $$('.ng-option');
    }
    public  selectedListField() {
        return $('.ng-value .ng-value-label');
    }

    public saveSettings() {
        const saveSettingsBtn = $('#saveSettingsBtn');
        saveSettingsBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }
}

const customersSettingsPage = new CustomersSettingsPage();
export default customersSettingsPage;
