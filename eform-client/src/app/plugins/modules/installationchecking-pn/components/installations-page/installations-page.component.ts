import {Component, OnInit, ViewChild} from '@angular/core';
import {PluginClaimsHelper} from '../../../../../common/helpers';
import {SharedPnService} from '../../../shared/services';
import {InstallationCheckingPnClaims} from '../../const';
import {PageSettingsModel} from '../../../../../common/models/settings';
import {InstallationsService} from '../../services';
import {InstallationModel, InstallationsListModel, InstallationsRequestModel} from '../../models';
import {InstallationRetractComponent} from '../installation-retract/installation-retract.component';
import {InstallationNewComponent} from '../installation-new/installation-new.component';
import {InstallationAssignComponent} from '../installation-assign/installation-asign.component';

@Component({
  selector: 'app-installations-page',
  templateUrl: './installations-page.component.html',
  styleUrls: ['./installations-page.component.scss']
})
export class InstallationsPageComponent implements OnInit {
  @ViewChild('newInstallationModal') newInstallationModal: InstallationNewComponent;
  @ViewChild('assignInstallationModal') assignInstallationModal: InstallationAssignComponent;
  @ViewChild('retractInstallationModal') retractInstallationModal: InstallationRetractComponent;
  localPageSettings: PageSettingsModel = new PageSettingsModel();
  installationsRequestModel: InstallationsRequestModel = new InstallationsRequestModel();
  installationsListModel: InstallationsListModel = new InstallationsListModel();
  spinnerStatus = false;


  get pluginClaimsHelper() {
    return PluginClaimsHelper;
  }

  get installationCheckingPnClaims() {
    return InstallationCheckingPnClaims;
  }

  constructor(
    private sharedPnService: SharedPnService,
    private installationsService: InstallationsService
  ) { }

  ngOnInit() {
    this.getLocalPageSettings();
  }

  getLocalPageSettings() {
    this.localPageSettings = this.sharedPnService
      .getLocalPageSettings('installationCheckingPnSettings', 'Installations')
      .settings;
    this.getInstallationsList();
  }

  updateLocalPageSettings() {
    this.sharedPnService.updateLocalPageSettings(
      'installationCheckingPnSettings',
      this.localPageSettings,
      'NotificationRules'
    );
    this.getInstallationsList();
  }

  getInstallationsList() {
    this.spinnerStatus = true;
    this.installationsRequestModel.isSortDsc = this.localPageSettings.isSortDsc;
    this.installationsRequestModel.sort = this.localPageSettings.sort;
    this.installationsRequestModel.pageSize = this.localPageSettings.pageSize;

    this.installationsService.getList(this.installationsRequestModel).subscribe((data) => {
      if (data && data.success) {
        this.installationsListModel = data.model;
      }
      this.spinnerStatus = false;
    });
  }

  showNewInstallationModal(id?: number) {
    this.newInstallationModal.show(id);
  }

  showAssignInstallationModal(id?: number) {
    this.assignInstallationModal.show(id);
  }

  showRetractInstallationModal(installationModel: InstallationModel) {
    this.assignInstallationModal.show(installationModel);
  }

  sortTable(sort: string) {
    if (this.localPageSettings.sort === sort) {
      this.localPageSettings.isSortDsc = !this.localPageSettings.isSortDsc;
    } else {
      this.localPageSettings.isSortDsc = false;
      this.localPageSettings.sort = sort;
    }
    this.updateLocalPageSettings();
  }

  changePage(e: any) {
    if (e || e === 0) {
      this.installationsRequestModel.offset = e;
      this.getInstallationsList();
    }
  }

  onSearchInputChanged(e: any) {
    this.installationsRequestModel.searchString = e.target.value;
    this.getInstallationsList();
  }
}
