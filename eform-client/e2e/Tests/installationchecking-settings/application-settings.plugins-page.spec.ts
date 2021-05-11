import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginsPage from './application-settings.plugins.page';

import {expect} from 'chai';

describe('Application settings page - site header section', function () {
  before(function () {
    loginPage.open('/auth');
  });
  it('should go to plugin settings page', function () {
    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    $('#spinner-animation').waitForDisplayed({timeout: 10000, reverse: true});
    pluginsPage.pluginName.waitForDisplayed({timeout: 50000});

    const pluginOne = pluginsPage.getFirstPluginRowObj();
    // expect(plugin.id).equal(1);
    if (pluginOne.name === 'Microting InstallationChecking Plugin') {
      expect(pluginOne.name).equal('Microting InstallationChecking Plugin');
    } else {
      expect(pluginOne.name).equal('Microting Customers Plugin');
    }
    expect(pluginOne.version).equal('1.0.0.0');


    const pluginTwo = pluginsPage.getSecondPluginRowObj();
    if (pluginTwo.name === 'Microting InstallationChecking Plugin') {
      expect(pluginTwo.name).equal('Microting InstallationChecking Plugin');
    } else {
      expect(pluginTwo.name).equal('Microting Customers Plugin');
    }
    expect(pluginTwo.version).equal('1.0.0.0');

  });
  it('should activate the customer plugin and installationChecking plugin', function () {
    const spinnerAnimation = $('#spinner-animation');
    let firstPlugin = pluginsPage.getFirstPluginRowObj();
    firstPlugin.activatePlugin();

    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    spinnerAnimation.waitForDisplayed({timeout: 10000, reverse: true});
    pluginsPage.pluginName.waitForDisplayed({timeout: 50000});

    let secondPlugin = pluginsPage.getSecondPluginRowObj();
    secondPlugin.activatePlugin();

    loginPage.login();
    myEformsPage.Navbar.goToPluginsPage();
    pluginsPage.pluginName.waitForDisplayed({timeout: 50000});
    spinnerAnimation.waitForDisplayed({timeout: 10000, reverse: true});

    firstPlugin = pluginsPage.getFirstPluginRowObj();
    secondPlugin = pluginsPage.getSecondPluginRowObj();


    expect(firstPlugin.status, 'status is not equal').eq(true);
    expect(secondPlugin.status, 'status is not equal').eq(true);
  });
});
