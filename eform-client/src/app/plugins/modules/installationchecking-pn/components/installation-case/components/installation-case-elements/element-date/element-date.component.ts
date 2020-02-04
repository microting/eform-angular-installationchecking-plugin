import {Component, Input} from '@angular/core';
import {FieldValueDto} from 'src/app/common/models';

@Component({
  selector: 'element-date',
  templateUrl: './element-date.component.html',
  styleUrls: ['./element-date.component.scss']
})
export class ElementDateComponent {
  fieldValueObj: FieldValueDto = new FieldValueDto();

  constructor() {
  }

  @Input()
  get fieldValue() {
    return this.fieldValueObj;
  }

  set fieldValue(val) {
    this.fieldValueObj = val;
  }
}
