import {Component, EventEmitter, Input, OnInit, Output, QueryList, ViewChildren} from '@angular/core';
import {EformReportDataItem, EformReportElement, ElementDto} from 'src/app/common/models';
import {UUID} from 'angular2-uuid';

@Component({
  selector: 'app-installation-case-block',
  templateUrl: './installation-case-block.component.html',
  styleUrls: ['./installation-case-block.component.scss']
})
export class InstallationCaseBlockComponent implements OnInit {
  @Input() element: ElementDto = new ElementDto();

  constructor() { }

  ngOnInit() {
  }
}
