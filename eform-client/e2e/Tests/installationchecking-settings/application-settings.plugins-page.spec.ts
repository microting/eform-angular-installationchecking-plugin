import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginPage from '../../Page objects/Plugin.page';

import {expect} from 'chai';

describe('Application settings page - site header section', function () {
  before(function () {
    loginPage.open('/auth');
  });
  it('should go to plugin settings page', function () {
    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    $('#spinner-animation').waitForDisplayed({timeout: 10000, reverse: true});

    const pluginOne = pluginPage.getFirstPluginRowObj();
    // expect(plugin.id).equal(1);
    if (pluginOne.name === 'Microting InstallationChecking Plugin') {
      expect(pluginOne.name).equal('Microting InstallationChecking Plugin');
    } else {
      expect(pluginOne.name).equal('Microting Customers Plugin');
    }
    expect(pluginOne.version).equal('1.0.0.0');


    const pluginTwo = pluginPage.getFirstPluginRowObj();
    if (pluginTwo.name === 'Microting InstallationChecking Plugin') {
      expect(pluginTwo.name).equal('Microting InstallationChecking Plugin');
    } else {
      expect(pluginTwo.name).equal('Microting Customers Plugin');
    }
    expect(pluginTwo.version).equal('1.0.0.0');

  });
  it('should activate the customer plugin and installationChecking plugin', function () {
    const spinnerAnimation = $('#spinner-animation');
    let firstPlugin = pluginPage.getPluginRowObjByIndex(1);
    firstPlugin.enableOrDisablePlugin();

    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    spinnerAnimation.waitForDisplayed({timeout: 10000, reverse: true});
    pluginPage.pluginName.waitForDisplayed({timeout: 50000});

    let secondPlugin = pluginPage.getPluginRowObjByIndex(2);
    secondPlugin.enableOrDisablePlugin();

    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    pluginPage.pluginName.waitForDisplayed({timeout: 50000});
    spinnerAnimation.waitForDisplayed({timeout: 10000, reverse: true});

    firstPlugin = pluginPage.getPluginRowObjByIndex(1);
    secondPlugin = pluginPage.getPluginRowObjByIndex(2);


    expect(firstPlugin.status, 'status is not equal').eq(true);
    expect(secondPlugin.status, 'status is not equal').eq(true);
  });
});
