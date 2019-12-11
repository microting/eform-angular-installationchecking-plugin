import {expect} from 'chai';
import loginPage from '../../../Page objects/Login.page';
import installationPage from '../../../Page objects/InstallationChecking/InstallationChecking-Installation.page';
import {Guid} from 'guid-typescript';
import customersPage from '../../../Page objects/InstallationChecking/Customers.page';
import deviceUsers from '../../../Page objects/DeviceUsers.page';
import installationsPage from '../../../Page objects/InstallationChecking/InstallationChecking.page';

describe('Installation Checking - Installation - Retract', function () {
  const companyName = Guid.create().toString();
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    // Setup customer
    customersPage.configureSearchableList();
    customersPage.createCustomer(companyName);
    // Setup device user
    deviceUsers.createDeviceUserFromScratch();
    // Go to installation page
    installationsPage.goToInstallationsPage();
  });
  it('Should assign installation', function () {
    const name = Guid.create().toString();
    browser.waitForVisible('#createInstallationBtn', 30000);
    installationPage.createInstallation(name);
    const installation = installationPage.getFirstRowObject();
    expect(installation.companyName).equal(name);
    browser.pause(8000);
    browser.refresh();
  });
  it('Should retract installation', function () {
    const name = Guid.create().toString();
    browser.waitForVisible('#createInstallationBtn', 30000);
    installationPage.createInstallation(name);
    const installation = installationPage.getFirstRowObject();
    expect(installation.companyName).equal(name);
    browser.pause(8000);
    browser.refresh();
  });
  it('should not retract installation', function () {
    browser.pause(8000);
    browser.waitForVisible('#retractInstallationBtn', 30000);
    installationPage.assignInstallation_Cancels();
    expect(installationPage.rowNum).equal(1);
  });
});
