import {expect} from 'chai';
import loginPage from '../../../Page objects/Login.page';
import installationPage from '../../../Page objects/InstallationChecking/InstallationChecking-Installation.page';
import {Guid} from 'guid-typescript';
import customersPage from '../../../Page objects/InstallationChecking/Customers.page';
import deviceUsers from '../../../Page objects/DeviceUsers.page';
import installationsPage from '../../../Page objects/InstallationChecking/InstallationChecking.page';
import myEformsPage from '../../../Page objects/MyEforms.page';

describe('Installation Checking - Installation - Add', function () {
  const companyName = 'BMW';
  const listName = 'My testing list';
  const deviceUserFirstName = 'John';
  const deviceUserLastName = 'Smith';
  const deviceUserFullName = `${deviceUserFirstName} ${deviceUserLastName}`;
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
    // Setup customer
    //customersPage.configureSearchableList(listName);
    customersPage.createCustomer(companyName);
    // // Setup device user

    myEformsPage.Navbar.goToDeviceUsersPage();
    deviceUsers.createNewDeviceUser(deviceUserFirstName, deviceUserLastName);
    // Go to installation page
    installationsPage.goToInstallationsPage();
  });
  it('should create installation', function () {
    $('#createInstallationBtn').waitForDisplayed({timeout: 30000});
    installationPage.createInstallation(companyName);
    const installation = installationPage.getFirstRowObject();
    expect(installation.companyName).equal(companyName);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    browser.refresh();
  });
  it('should not create installation', function () {
    const rowNumsBeforeCreate = installationPage.rowNum;
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    $('#createInstallationBtn').waitForDisplayed({timeout: 30000});
    installationPage.createInstallation_Cancels();
    expect(rowNumsBeforeCreate).equal(installationPage.rowNum);
  });
  it('should not assign installation', function () {
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const installation = installationPage.getFirstRowObject();
    const checkboxBeforeAssign = installation.assignCheckbox;
    $('#installationAssignBtn').waitForDisplayed({timeout: 30000});
    installationPage.assignInstallation_Cancels();
    expect(installation.assignCheckbox).equal(checkboxBeforeAssign);
  });
  it('should assign installation', function () {
    $('#installationAssignBtn').waitForDisplayed({timeout: 30000});
    installationPage.assignInstallation(deviceUserFullName);
    $('#installationAssignBtn').waitForDisplayed({timeout: 30000});
    const installation = installationPage.getFirstRowObject();
    expect(installation.assignedTo).equal(deviceUserFullName);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    //browser.refresh();
  });
  it('should not retract installation', function () {
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    installationPage.retractInstallation_Cancels();
    const installation = installationPage.getFirstRowObject();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    expect(installation.assignedTo).equal(deviceUserFullName);
  });
  it('should retract installation', function () {
      $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
      installationPage.retractInstallation();
      const installation = installationPage.getFirstRowObject();
      $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
      expect(installation.assignedTo).equal('');
  });
});
