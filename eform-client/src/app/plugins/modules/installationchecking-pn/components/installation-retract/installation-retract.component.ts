import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {InstallationModel} from '../../models';
import {InstallationsService} from '../../services';

@Component({
  selector: 'app-installation-retract',
  templateUrl: './installation-retract.component.html',
  styleUrls: ['./installation-retract.component.scss']
})
export class InstallationRetractComponent implements OnInit {
  @ViewChild('frame', {static: false}) frame;
  @Output() installationRetracted: EventEmitter<void> = new EventEmitter<void>();
  selectedModel: InstallationModel = new InstallationModel();

  constructor(private installationsService: InstallationsService) { }

  ngOnInit() {
  }

  show(selectedModel: InstallationModel) {
    this.selectedModel = selectedModel;
    this.frame.show();
  }

  retractInstallation() {

    this.installationsService.retract(this.selectedModel.id).subscribe((data) => {
      if (data && data.success) {
        this.frame.hide();
        this.installationRetracted.emit();
      }
    });
  }
}
