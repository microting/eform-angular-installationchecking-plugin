import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginPage from '../../Page objects/Plugin.page';

import {expect} from 'chai';
import pluginsPage from './application-settings.plugins.page';

describe('Application settings page - site header section', function () {
    before(function () {
        loginPage.open('/auth');
    });
    it('should go to plugin settings page', function () {
        loginPage.login();
        myEformsPage.Navbar.advancedDropdown();
        myEformsPage.Navbar.clickonSubMenuItem('Plugins');
        $('#plugin-name').waitForDisplayed(50000);
        $('#spinner-animation').waitForDisplayed(10000, true);

        const plugin = pluginsPage.getFirstPluginRowObj();
        expect(plugin.id).equal(1);
        if (plugin.name === 'Microting InstallationChecking Plugin') {
            expect(plugin.name).equal('Microting InstallationChecking Plugin');
        } else {
            expect(plugin.name).equal('Microting Customers Plugin');
        }
        expect(plugin.version).equal('1.0.0.0');

        const pluginTwo = pluginsPage.getSecondPluginRowObj();
        expect(pluginTwo.id).equal(2);
        if (pluginTwo.name === 'Microting InstallationChecking Plugin') {
            expect(pluginTwo.name).equal('Microting InstallationChecking Plugin');
        } else {
            expect(pluginTwo.name).equal('Microting Customers Plugin');
        }
        expect(pluginTwo.version).equal('1.0.0.0');

    });
    it('should activate the customer plugin', function () {
        const plugin = pluginsPage.getFirstPluginRowObj();
        // pluginPage.pluginSettingsBtn.click();
        plugin.activateBtn.click();
        $('#pluginOKBtn').waitForDisplayed(40000);
        pluginPage.pluginOKBtn.click();
        browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
        loginPage.open('/');

        loginPage.login();
        myEformsPage.Navbar.advancedDropdown();
        myEformsPage.Navbar.clickonSubMenuItem('Plugins');
        $('#plugin-name').waitForDisplayed(50000);
        $('#spinner-animation').waitForDisplayed(10000, true);

        const secondPlugin = pluginsPage.getSecondPluginRowObj();
        expect(secondPlugin.id).equal(2);
        // expect(secondPlugin.name).equal('Microting Customers Plugin');
        expect(secondPlugin.version).equal('1.0.0.0');

        // pluginPage.pluginSettingsBtn.click();
        secondPlugin.activateBtn.click();
        $('#pluginOKBtn').waitForDisplayed(40000);
        pluginPage.pluginOKBtn.click();
        browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
        loginPage.open('/');

        loginPage.login();
        myEformsPage.Navbar.advancedDropdown();
        myEformsPage.Navbar.clickonSubMenuItem('Plugins');
        $('#plugin-name').waitForDisplayed(50000);
        $('#spinner-animation').waitForDisplayed(10000, true);
        // browser.pause(10000);

        const pluginToFind = pluginsPage.getFirstPluginRowObj();
        expect(pluginToFind.id).equal(1);
        // expect(pluginToFind.name).equal('Microting InstallationChecking Plugin');
        expect(pluginToFind.version).equal('1.0.0.0');
        $(`//*[contains(text(), 'Planlægning')]`).waitForDisplayed(20000);
        $(`//*[contains(text(), 'Kunder')]`).waitForDisplayed(20000);
    });

    // it('should activate the plugin', function () {
    //   let plugin = pluginsPage.getFirstPluginRowObj();
    //   plugin.activateBtn.click();
    //   $('#pluginOKBtn').waitForDisplayed(40000);
    //   pluginPage.pluginOKBtn.click();
    //   browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
    //   browser.refresh();
    //
    //   loginPage.login();
    //   myEformsPage.Navbar.advancedDropdown();
    //   myEformsPage.Navbar.clickonSubMenuItem('Plugins');
    //   $('#plugin-name').waitForDisplayed(50000);
    //   browser.pause(10000);
    //
    //   const pluginTwo = pluginsPage.getSecondPluginRowObj();
    //   pluginTwo.activateBtn.click();
    //   $('#pluginOKBtn').waitForDisplayed(40000);
    //   pluginPage.pluginOKBtn.click();
    //   browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
    //   browser.refresh();
    //
    //   loginPage.login();
    //   myEformsPage.Navbar.advancedDropdown();
    //   myEformsPage.Navbar.clickonSubMenuItem('Plugins');
    //   $('#plugin-name').waitForDisplayed(50000);
    //   browser.pause(10000);
    //   browser.pause(10000);
    //
    //   plugin = pluginsPage.getFirstPluginRowObj();
    //   expect(plugin.id).equal(1);
    //   expect(plugin.name).equal('Microting InstallationChecking Plugin');
    //   expect(plugin.version).equal('1.0.0.0');
    //
    //
    //   expect(pluginTwo.id).equal(2);
    //   expect(pluginTwo.name).equal('Microting Customers Plugin');
    //   expect(pluginTwo.version).equal('1.0.0.0');
    //   expect(browser.element(`//*[contains(@class, 'dropdown')]//*[contains(text(), 'Planlægning')]`).isExisting()).equal(true);
    // });
});
