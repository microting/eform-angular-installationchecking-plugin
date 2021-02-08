import {PageWithNavbarPage} from '../../Page objects/PageWithNavbar.page';
import pluginPage from '../../Page objects/Plugin.page';

class ApplicationSettingsPluginsPage extends PageWithNavbarPage {
  constructor() {
    super();
  }

  getFirstPluginRowObj(): PluginRowObject {
    browser.pause(500);
    return new PluginRowObject(1);
  }

  getSecondPluginRowObj(): PluginRowObject {
    browser.pause(500);
    return new PluginRowObject(2);
  }

  public get pluginName() {
    const ele = $('#plugin-name');
    ele.waitForDisplayed({timeout: 20000});
    return ele;
  }
}


const pluginsPage = new ApplicationSettingsPluginsPage();
export default pluginsPage;

class PluginRowObject {
  constructor(rowNum) {
    if ($$('#plugin-id')[rowNum - 1]) {
      this.id = +$$('#plugin-id')[rowNum - 1].getText();
      this.name = $$('#plugin-name')[rowNum - 1].getText();
      this.version = $$('#plugin-version')[rowNum - 1].getText();
      this.status = $$('#plugin-status')[rowNum - 1].getText();
      this.settingsBtn = $$('#plugin-settings-btn')[rowNum - 1];
      this.activateBtn = $$(`#plugin-status button`)[rowNum - 1];
    }
  }

  id: number;
  name: string;
  version: string;
  status;
  settingsBtn;
  activateBtn;

  activatePlugin() {
    this.activateBtn.click();
    $('#pluginOKBtn').waitForDisplayed({timeout: 40000});
    pluginPage.pluginOKBtn.click();
    browser.pause(60000); // We need to wait 60 seconds for the plugin to create db etc.
  }
}
