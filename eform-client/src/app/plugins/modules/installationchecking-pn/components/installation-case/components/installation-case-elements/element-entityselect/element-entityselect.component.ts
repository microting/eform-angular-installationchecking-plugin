import {AfterViewInit, Component, Input, OnInit} from '@angular/core';
import {FieldValueDto} from 'src/app/common/models';
import {CommonDictionaryTextModel} from 'src/app/common/models/common';

@Component({
  selector: 'element-entityselect',
  templateUrl: './element-entityselect.component.html',
  styleUrls: ['./element-entityselect.component.scss']
})
export class ElementEntityselectComponent implements OnInit, AfterViewInit {
  items: Array<CommonDictionaryTextModel> = [];
  fieldValueObj: FieldValueDto = new FieldValueDto();
  @Input() entityGroupUid: string;

  @Input()
  get fieldValue() {
    return this.fieldValueObj;
  }

  set fieldValue(val) {
    this.fieldValueObj = {...val, valueReadable: val.valueReadable ? val.valueReadable : ''};
  }

  constructor() {
  }

  ngOnInit() {

  }

  ngAfterViewInit(): void {

  }
}
