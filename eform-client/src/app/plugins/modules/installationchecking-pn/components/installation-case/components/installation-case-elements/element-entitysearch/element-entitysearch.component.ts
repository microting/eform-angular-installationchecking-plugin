import {AfterViewInit, Component, Input} from '@angular/core';
import {FieldValueDto} from 'src/app/common/models';
import {CommonDictionaryTextModel} from 'src/app/common/models/common';

@Component({
  selector: 'element-entitysearch',
  templateUrl: './element-entitysearch.component.html',
  styleUrls: ['./element-entitysearch.component.scss']
})
export class ElementEntitysearchComponent implements AfterViewInit {
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

  onSelectedChanged(e: any) {
    this.fieldValue.value = e.id;
  }

  ngAfterViewInit(): void {
    if (this.fieldValueObj.valueReadable === 'null') {
      this.fieldValueObj.valueReadable = '';
    }
  }
}
