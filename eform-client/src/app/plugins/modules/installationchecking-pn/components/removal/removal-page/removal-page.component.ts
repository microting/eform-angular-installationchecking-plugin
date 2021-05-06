import {
  InstallationCheckingPnClaims,
  InstallationStateEnum,
} from '../../../const';
import { Paged, TableHeaderElementModel } from 'src/app/common/models';
import { AuthStateService } from 'src/app/common/store';
import { InstallationModel } from '../../../models';
import { ToastrService } from 'ngx-toastr';
import { InstallationsService } from '../../../services';
import { Component, OnInit, ViewChild } from '@angular/core';
import {
  InstallationAssignComponent,
  InstallationRetractComponent,
} from '../../../components';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { PluginClaimsHelper } from 'src/app/common/helpers';
import { RemovalStateService } from '../store';
import { TranslateService } from '@ngx-translate/core';
import moment = require('moment');

@Component({
  selector: 'app-removal-page',
  templateUrl: './removal-page.component.html',
  styleUrls: ['./removal-page.component.scss'],
})
export class RemovalPageComponent implements OnInit {
  @ViewChild('assignInstallationModal', { static: false })
  assignInstallationModal: InstallationAssignComponent;
  @ViewChild('retractInstallationModal', { static: false })
  retractInstallationModal: InstallationRetractComponent;
  installationsListModel: Paged<InstallationModel> = new Paged<InstallationModel>();
  states = [
    {
      id: InstallationStateEnum.NotAssigned,
      label: this.translateService.instant('Not assigned'),
    },
    {
      id: InstallationStateEnum.Assigned,
      label: this.translateService.instant('Assigned'),
    },
    {
      id: InstallationStateEnum.Completed,
      label: this.translateService.instant('Completed'),
    },
    {
      id: InstallationStateEnum.Archived,
      label: this.translateService.instant('Archived'),
    },
  ];
  searchSubject = new Subject();

  tableHeaders: TableHeaderElementModel[] = [
    { name: 'Id', elementId: 'idSort', sortable: true },
    {
      name: 'CompanyName',
      elementId: 'companyNameSort',
      sortable: true,
      visibleName: 'Company name',
    },
    {
      name: 'CompanyAddress',
      elementId: 'companyAddressSort',
      sortable: true,
      visibleName: 'Company address',
    },
    {
      name: 'CompanyAddress2',
      elementId: 'companyAddress2Sort',
      sortable: true,
      visibleName: 'Company address 2',
    },
    {
      name: 'ZipCode',
      elementId: 'zipCodeSort',
      sortable: true,
      visibleName: 'Zip code',
    },
    {
      name: 'CityName',
      elementId: 'cityNameSort',
      sortable: true,
      visibleName: 'City name',
    },
    {
      name: 'CountryCode',
      elementId: 'countryCodeSort',
      sortable: true,
      visibleName: 'Country code',
    },
    {
      name: 'DateInstall',
      elementId: 'dateInstallSort',
      sortable: true,
      visibleName: 'Date install',
    },
    {
      name: 'DateRemove',
      elementId: 'dateRemoveSort',
      sortable: true,
      visibleName: 'Date remove',
    },
    {
      name: 'DateActRemove',
      elementId: 'dateActRemoveSort',
      sortable: true,
      visibleName: 'Date act remove',
    },
    {
      name: 'Assigned to',
      elementId: 'assignedToSort',
      sortable: false,
    },
    this.authStateService.checkClaim(
      this.installationCheckingPnClaims.assignInstallations
    )
      ? {
          name: 'Assign',
          elementId: 'assignSort',
          sortable: false,
        }
      : null,
    { name: 'Actions', elementId: '', sortable: false },
  ];

  get pluginClaimsHelper() {
    return PluginClaimsHelper;
  }

  get installationCheckingPnClaims() {
    return InstallationCheckingPnClaims;
  }

  get installationStates() {
    return InstallationStateEnum;
  }

  constructor(
    private translateService: TranslateService,
    private toastrService: ToastrService,
    private installationsService: InstallationsService,
    public authStateService: AuthStateService,
    public removalStateService: RemovalStateService
  ) {
    this.searchSubject.pipe(debounceTime(500)).subscribe((val: string) => {
      this.removalStateService.updateNameFilter(val);
      this.getRemovalsList();
    });
  }

  ngOnInit() {
    this.getRemovalsList();
  }

  getRemovalsList() {
    this.removalStateService.getList().subscribe((data) => {
      if (data && data.success) {
        this.installationsListModel = data.model;
      }
    });
  }

  archiveInstallation(id: number) {
    this.installationsService.archive(id).subscribe((data) => {
      if (data && data.success) {
        this.getRemovalsList();
      }
    });
  }

  showAssignInstallationModal() {
    const installationIds = this.installationsListModel.entities
      .filter((x) => x.assign)
      .map((x) => x.id);
    this.assignInstallationModal.show(installationIds);
  }

  showRetractInstallationModal(installationModel: InstallationModel) {
    this.retractInstallationModal.show(installationModel);
  }

  sortTable(sort: string) {
    this.removalStateService.onSortTable(sort);
    this.getRemovalsList();
  }

  changePage(offset: number) {
    this.removalStateService.changePage(offset);
    this.getRemovalsList();
  }

  onSearchInputChanged(nameFilter: string) {
    this.searchSubject.next(nameFilter);
  }

  onSelectStateChanged(state: InstallationStateEnum) {
    this.removalStateService.updateStateFilter(state);
    this.getRemovalsList();
  }

  saveExcel(id: number) {
    this.installationsService.excel(id).subscribe(
      (data) => {
        saveAs(
          data,
          moment().format('YYYY-MM-DD') + '_installation_' + id + '.xlsx'
        );
      },
      (error) => {
        this.toastrService.error(error);
      }
    );
  }

  onPageSizeChanged(pageSize: number) {
    this.removalStateService.updatePageSize(pageSize);
    this.getRemovalsList();
  }
}
