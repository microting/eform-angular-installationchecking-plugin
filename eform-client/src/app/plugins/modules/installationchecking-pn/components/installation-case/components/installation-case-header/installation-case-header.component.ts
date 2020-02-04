import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {EformReportModel} from 'src/app/common/models/eforms/report';
import {InstallationModel} from '../../../../models';

@Component({
  selector: 'app-installation-case-header',
  templateUrl: './installation-case-header.component.html',
  styleUrls: ['./installation-case-header.component.scss']
})
export class InstallationCaseHeaderComponent implements OnInit {
  @Input() installationModel: InstallationModel = new InstallationModel();
  @ViewChild('reportCropperModal') reportCropperModal;
  constructor() { }

  ngOnInit() {
  }
}
