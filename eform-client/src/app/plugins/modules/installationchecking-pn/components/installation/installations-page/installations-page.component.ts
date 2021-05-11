import { Component, OnInit, ViewChild } from '@angular/core';
import {
  InstallationCheckingPnClaims,
  InstallationStateEnum,
} from '../../../const';
import { InstallationModel } from '../../../models';
import {
  InstallationAssignComponent,
  InstallationNewComponent,
  InstallationRetractComponent,
} from '../';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { Paged, TableHeaderElementModel } from 'src/app/common/models';
import { AuthStateService } from 'src/app/common/store';
import { InstallationStateService } from '../store';

@Component({
  selector: 'app-installations-page',
  templateUrl: './installations-page.component.html',
  styleUrls: ['./installations-page.component.scss'],
})
export class InstallationsPageComponent implements OnInit {
  @ViewChild('newInstallationModal', { static: false })
  newInstallationModal: InstallationNewComponent;
  @ViewChild('assignInstallationModal', { static: false })
  assignInstallationModal: InstallationAssignComponent;
  @ViewChild('retractInstallationModal', { static: false })
  retractInstallationModal: InstallationRetractComponent;
  installationsListModel: Paged<InstallationModel> = new Paged<InstallationModel>();
  states = [];
  searchSubject = new Subject();

  tableHeaders: TableHeaderElementModel[] = [
    { name: 'Id', elementId: 'idTableHeader', sortable: true },
    {
      name: 'CompanyName',
      elementId: 'companyNameTableHeader',
      sortable: true,
      visibleName: 'Company name',
    },
    {
      name: 'CompanyAddress',
      elementId: 'companyAddressTableHeader',
      sortable: true,
      visibleName: 'Company address',
    },
    {
      name: 'CompanyAddress2',
      elementId: 'companyAddress2TableHeader',
      sortable: true,
      visibleName: 'Company address 2',
    },
    {
      name: 'ZipCode',
      elementId: 'zipCodeTableHeader',
      sortable: true,
      visibleName: 'Zip code',
    },
    {
      name: 'CityName',
      elementId: 'cityNameTableHeader',
      sortable: true,
      visibleName: 'City name',
    },
    {
      name: 'CountryCode',
      elementId: 'countryCodeTableHeader',
      sortable: true,
      visibleName: 'Country code',
    },
    {
      name: 'DateInstall',
      elementId: 'dateInstallTableHeader',
      sortable: true,
      visibleName: 'Date install',
    },
    {
      name: 'Assigned to',
      elementId: 'assignedToTableHeader',
      sortable: false,
    },
    this.authStateService.checkClaim(
      this.installationCheckingPnClaims.assignInstallations
    )
      ? {
          name: 'Assign',
          elementId: 'assignTableHeader',
          sortable: false,
        }
      : null,
    { name: 'Actions', elementId: '', sortable: false },
  ];

  get installationCheckingPnClaims() {
    return InstallationCheckingPnClaims;
  }

  get installationStates() {
    return InstallationStateEnum;
  }

  get someAssign() {
    if (
      !this.installationsListModel.total ||
      this.installationsListModel.total === 0
    ) {
      return false;
    }
    return this.installationsListModel.entities.some((x) => x.assign);
  }

  constructor(
    translateService: TranslateService,
    public authStateService: AuthStateService,
    public installationsStateService: InstallationStateService
  ) {
    this.states = [
      {
        id: InstallationStateEnum.NotAssigned,
        label: translateService.instant('Not assigned'),
      },
      {
        id: InstallationStateEnum.Assigned,
        label: translateService.instant('Assigned'),
      },
      {
        id: InstallationStateEnum.Completed,
        label: translateService.instant('Completed'),
      },
    ];
    this.searchSubject.pipe(debounceTime(500)).subscribe((val: string) => {
      this.installationsStateService.updateNameFilter(val);
      this.getInstallationsList();
    });
  }

  ngOnInit() {
    this.getInstallationsList();
  }

  getInstallationsList() {
    this.installationsStateService.getList().subscribe((data) => {
      if (data && data.success) {
        this.installationsListModel = data.model;
      }
    });
  }

  showNewInstallationModal() {
    this.newInstallationModal.show();
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
    this.installationsStateService.onSortTable(sort);
    this.getInstallationsList();
  }

  changePage(offset: number) {
    this.installationsStateService.changePage(offset);
    this.getInstallationsList();
  }

  onSearchInputChanged(newNameFilter: string) {
    this.searchSubject.next(newNameFilter);
  }

  onSelectStateChanged(newState: InstallationStateEnum) {
    this.installationsStateService.updateStateFilter(newState);
    this.getInstallationsList();
  }

  onPageSizeChanged(pageSize: number) {
    this.installationsStateService.updatePageSize(pageSize);
    this.getInstallationsList();
  }
}
