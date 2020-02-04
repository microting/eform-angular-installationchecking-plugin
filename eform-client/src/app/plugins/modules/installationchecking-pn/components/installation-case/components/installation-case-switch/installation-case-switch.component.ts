import {Component, Input, OnInit} from '@angular/core';
import {DataItemDto} from 'src/app/common/models';

@Component({
  selector: 'app-installation-case-switch',
  templateUrl: './installation-case-switch.component.html',
  styleUrls: ['./installation-case-switch.component.scss']
})
export class InstallationCaseSwitchComponent implements OnInit {
  @Input() dataItemList: Array<DataItemDto> = [];
  constructor() { }

  ngOnInit() {
  }
}
