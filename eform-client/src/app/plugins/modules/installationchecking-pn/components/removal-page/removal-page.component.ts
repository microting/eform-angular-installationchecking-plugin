import {Component, OnInit, ViewChild} from '@angular/core';
import {InstallationAssignComponent, InstallationRetractComponent} from '..';
import {PageSettingsModel} from '../../../../../common/models/settings';
import {InstallationModel, InstallationsListModel, InstallationsRequestModel} from '../../models';
import {PluginClaimsHelper} from '../../../../../common/helpers';
import {InstallationCheckingPnClaims, InstallationsSortColumns, InstallationStateEnum, InstallationTypeEnum} from '../../const';
import {SharedPnService} from '../../../shared/services';
import {InstallationsService} from '../../services';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'app-removal-page',
  templateUrl: './removal-page.component.html',
  styleUrls: ['./removal-page.component.scss']
})
export class RemovalPageComponent implements OnInit {
  @ViewChild('assignInstallationModal') assignInstallationModal: InstallationAssignComponent;
  @ViewChild('retractInstallationModal') retractInstallationModal: InstallationRetractComponent;
  localPageSettings: PageSettingsModel = new PageSettingsModel();
  installationsRequestModel: InstallationsRequestModel = new InstallationsRequestModel();
  installationsListModel: InstallationsListModel = new InstallationsListModel();
  states = [];
  spinnerStatus = false;

  get pluginClaimsHelper() {
    return PluginClaimsHelper;
  }

  get installationCheckingPnClaims() {
    return InstallationCheckingPnClaims;
  }

  get sortCols() {
    return InstallationsSortColumns;
  }

  get installationStates() {
    return InstallationStateEnum;
  }

  constructor(
    private sharedPnService: SharedPnService,
    private translateService: TranslateService,
    private installationsService: InstallationsService
  ) {
    this.states = [
      { id: InstallationStateEnum.NotAssigned, label: translateService.instant('Not assigned') },
      { id: InstallationStateEnum.Assigned, label: translateService.instant('Assigned') },
      { id: InstallationStateEnum.Completed, label: translateService.instant('Completed') },
      { id: InstallationStateEnum.Archived, label: translateService.instant('Archived') }
    ];
  }

  ngOnInit() {
    this.getLocalPageSettings();
  }

  getLocalPageSettings() {
    this.localPageSettings = this.sharedPnService
      .getLocalPageSettings('installationCheckingPnSettings', 'Removals')
      .settings;
    this.getRemovalsList();
  }

  updateLocalPageSettings() {
    this.sharedPnService.updateLocalPageSettings(
      'installationCheckingPnSettings',
      this.localPageSettings,
      'Removals'
    );
    this.getRemovalsList();
  }

  getRemovalsList() {
    this.spinnerStatus = true;
    this.installationsRequestModel.isSortDsc = this.localPageSettings.isSortDsc;
    this.installationsRequestModel.sort = this.localPageSettings.sort;
    this.installationsRequestModel.pageSize = this.localPageSettings.pageSize;
    this.installationsRequestModel.type = InstallationTypeEnum.Removal;

    this.installationsService.getList(this.installationsRequestModel).subscribe((data) => {
      if (data && data.success) {
        this.installationsListModel = data.model;
      }
      this.spinnerStatus = false;
    });
  }

  archiveInstallation(id: number) {
    this.spinnerStatus = true;
    this.installationsService.archive(id).subscribe((data) => {
      if (data && data.success) {
        this.getRemovalsList();
      }
      this.spinnerStatus = false;
    });
  }

  showAssignInstallationModal() {
    const installationIds = this.installationsListModel.installations.map(x => x.id);
    this.assignInstallationModal.show(installationIds);
  }

  showRetractInstallationModal(installationModel: InstallationModel) {
    this.retractInstallationModal.show(installationModel);
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
      this.getRemovalsList();
    }
  }

  onSearchInputChanged(e: any) {
    this.installationsRequestModel.searchString = e.target.value;
    this.getRemovalsList();
  }

  getSortIcon(sort: string): string {
    if (this.installationsRequestModel.sort === sort) {
      return this.installationsRequestModel.isSortDsc ? 'expand_more' : 'expand_less';
    } else {
      return 'unfold_more';
    }
  }
}
