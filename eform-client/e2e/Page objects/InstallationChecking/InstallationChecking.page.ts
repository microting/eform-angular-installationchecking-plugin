import Page from '../Page';

export class InstallationsPage extends Page {
    constructor() {
        super();
    }
    public get rowNum(): number {
        browser.pause(500);
        return $$('#tableBody > tr').length;
    }
    public InstallationCheckingDropDown() {
        $(`//*[contains(@class, 'dropdown')]//*[contains(text(), 'Planlægning')]`).click();
    }
    public get InstallationsBtn() {
        $('#installationchecking-pn-installation').waitForDisplayed({timeout: 20000});
        $('#installationchecking-pn-installation').waitForClickable({timeout: 20000});
        return $('#installationchecking-pn-installation');
    }
    goToInstallationsPage() {
        this.InstallationCheckingDropDown();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
        this.InstallationsBtn.click();
        $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    }
}

const installationsPage = new InstallationsPage();
export default installationsPage;
