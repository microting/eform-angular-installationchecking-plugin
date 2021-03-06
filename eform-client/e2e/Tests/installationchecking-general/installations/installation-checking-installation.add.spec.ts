import {expect} from 'chai';
import loginPage from '../../../Page objects/Login.page';
import installationPage from '../../../Page objects/InstallationChecking/InstallationChecking-Installation.page';
import {Guid} from 'guid-typescript';
import customersPage from '../../../Page objects/InstallationChecking/Customers.page';
import deviceUsers from '../../../Page objects/DeviceUsers.page';
import installationsPage from '../../../Page objects/InstallationChecking/InstallationChecking.page';

describe('Installation Checking - Installation - Add', function () {
  const companyName = Guid.create().toString();
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    // Setup customer
    customersPage.createCustomer(companyName);
    // Setup device user
    deviceUsers.createNewDeviceUser();
    // Go to installation page
    installationsPage.goToInstallationsPage();
  });
  it('Should create installation', function () {
    $('#createInstallationBtn').waitForDisplayed({timeout: 30000});
    installationPage.createInstallation(companyName);
    const installation = installationPage.getFirstRowObject();
    expect(installation.companyName).equal(companyName);
  });
  it('should not create installation', function () {
    const rowNumsBeforeCreate = installationPage.rowNum;
    browser.pause(8000);
    $('#createInstallationBtn').waitForDisplayed({timeout: 30000});
    installationPage.createInstallation_Cancels();
    expect(rowNumsBeforeCreate).equal(installationPage.rowNum);
  });
});
