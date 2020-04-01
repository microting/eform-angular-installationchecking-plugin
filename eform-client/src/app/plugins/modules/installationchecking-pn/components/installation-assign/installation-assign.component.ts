import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {InstallationsService} from '../../services';
import {DeviceUserService} from '../../../../../common/services/device-users';
import {InstallationsAssignModel} from '../../models';

@Component({
  selector: 'app-installation-assign',
  templateUrl: './installation-assign.component.html',
  styleUrls: ['./installation-assign.component.scss']
})
export class InstallationAssignComponent implements OnInit {
  @ViewChild('frame', {static: false}) frame;
  @Output() installationAssigned: EventEmitter<void> = new EventEmitter<void>();
  sites = [];
  installationIds: number[];
  selectedSiteId: number;

  spinnerStatus = false;

  constructor(
    private installationsService: InstallationsService,
    private deviceUserService: DeviceUserService
  ) { }

  ngOnInit() {
    this.deviceUserService.getAllDeviceUsers().subscribe(data => {
      if (data && data.success) {
        this.sites = data.model.map(x => ({
          id: x.siteId,
          label: x.firstName + ' ' + x.lastName
        }));
      }
    });
  }

  show(installationIds: number[]) {
    this.installationIds = installationIds;
    this.frame.show();
  }

  assignInstallation() {
    this.spinnerStatus = true;
    const assignModel = { employeeId: this.selectedSiteId, installationIds: this.installationIds } as InstallationsAssignModel;

    this.installationsService.assign(assignModel).subscribe((data) => {
      if (data && data.success) {
        this.frame.hide();
        this.installationAssigned.emit();
      }
      this.spinnerStatus = false;
    });
  }
}
