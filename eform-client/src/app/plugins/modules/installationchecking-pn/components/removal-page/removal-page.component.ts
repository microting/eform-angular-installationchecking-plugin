import {Component, OnInit, ViewChild} from '@angular/core';
import {InstallationAssignComponent, InstallationRetractComponent} from '..';
import {PageSettingsModel} from '../../../../../common/models/settings';
import {InstallationModel, InstallationsListModel, InstallationsRequestModel} from '../../models';
import {PluginClaimsHelper} from '../../../../../common/helpers';
import {InstallationCheckingPnClaims, InstallationsSortColumns, InstallationStateEnum, InstallationTypeEnum} from '../../const';
import {SharedPnService} from '../../../shared/services';
import {InstallationsService} from '../../services';
import {TranslateService} from '@ngx-translate/core';
import {saveAs} from 'file-saver';
import * as moment from 'moment';
import {ToastrService} from 'ngx-toastr';
import {debounceTime} from 'rxjs/operators';
import {Subject} from 'rxjs';

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
  searchSubject = new Subject();

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
    private toastrService: ToastrService,
    private installationsService: InstallationsService
  ) {
    this.states = [
      { id: InstallationStateEnum.NotAssigned, label: translateService.instant('Not assigned') },
      { id: InstallationStateEnum.Assigned, label: translateService.instant('Assigned') },
      { id: InstallationStateEnum.Completed, label: translateService.instant('Completed') },
      { id: InstallationStateEnum.Archived, label: translateService.instant('Archived') }
    ];
    this.searchSubject.pipe(
      debounceTime(500)
    ). subscribe(val => {
      this.installationsRequestModel.searchString =  val.toString();
      this.getRemovalsList();
    });
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
    const installationIds = this.installationsListModel.installations.filter(x => x.assign).map(x => x.id);
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
    this.searchSubject.next(e.target.value);
  }

  onSelectStateChanged(e: number) {
    this.installationsRequestModel.state = e;
    this.getRemovalsList();
  }

  saveExcel(id: number) {
    this.spinnerStatus = true;
    this.installationsService.excel(id).subscribe(((data) => {
      saveAs(data, moment().format('YYYY-MM-DD') + '_installation_' + id + '.xlsx');
      this.spinnerStatus = false;
    }), error => {
      this.toastrService.error(error);
      this.spinnerStatus = false;
    });
  }

  getSortIcon(sort: string): string {
    if (this.installationsRequestModel.sort === sort) {
      return this.installationsRequestModel.isSortDsc ? 'expand_more' : 'expand_less';
    } else {
      return 'unfold_more';
    }
  }
}
