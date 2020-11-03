import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {DataItemDto} from 'src/app/common/models';
import {Subscription} from 'rxjs';
import {TemplateFilesService} from 'src/app/common/services';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';

@AutoUnsubscribe()
@Component({
  selector: 'element-pdf',
  templateUrl: './element-pdf.component.html',
  styleUrls: ['./element-pdf.component.scss']
})
export class ElementPdfComponent implements OnDestroy {
  dataItemObj: DataItemDto = new DataItemDto();
  pdfSub$: Subscription;

  @Input()
  get dataItem() {
    return this.dataItem;
  }

  set dataItem(val) {
    this.dataItemObj = val;
  }

  constructor(private templateFilesService: TemplateFilesService) {
  }

  ngOnDestroy(): void {
  }

  getPdf(fileName: string) {
    // TODO: CHECK
    this.pdfSub$ = this.templateFilesService.getPdfFile(fileName).subscribe((blob) => {
      const fileURL = URL.createObjectURL(blob);
      window.open(fileURL, '_blank');
    });
  }
}
