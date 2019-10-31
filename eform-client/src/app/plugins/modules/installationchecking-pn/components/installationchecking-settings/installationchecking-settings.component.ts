import {Component, OnInit} from '@angular/core';
import {InstallationCheckingPnSettingsService} from '../../services';
import {InstallationCheckingBaseSettingsModel} from '../../models';

@Component({
  selector: 'app-installationchecking-settings',
  templateUrl: './installationchecking-settings.component.html',
  styleUrls: ['./installationchecking-settings.component.scss']
})
export class InstallationCheckingSettingsComponent implements OnInit {
  spinnerStatus = false;
  settingsModel: InstallationCheckingBaseSettingsModel = new InstallationCheckingBaseSettingsModel();

  constructor(private installationcheckingPnSettingsService: InstallationCheckingPnSettingsService) {
  }

  ngOnInit() {
    this.getSettings();
  }


  getSettings() {
    this.spinnerStatus = true;
    this.installationcheckingPnSettingsService.getAllSettings().subscribe((data) => {
      if (data && data.success) {
        this.settingsModel = data.model;
      } this.spinnerStatus = false;
    });
  }

  updateSettings() {
    this.spinnerStatus = true;
    this.installationcheckingPnSettingsService.updateSettings(this.settingsModel)
      .subscribe((data) => {
        if (data && data.success) {

        } this.spinnerStatus = false;
      });
  }
}
