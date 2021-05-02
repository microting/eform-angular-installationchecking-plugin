import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { debounceTime, switchMap } from 'rxjs/operators';
import {
  CustomerPnModel,
  CustomersPnRequestModel,
} from '../../../../customers-pn/models/customer';
import { InstallationsService } from '../../../services';
import {CustomersPnService} from 'src/app/plugins/modules/customers-pn/services';

@Component({
  selector: 'app-installation-new',
  templateUrl: './installation-new.component.html',
  styleUrls: ['./installation-new.component.scss'],
})
export class InstallationNewComponent implements OnInit {
  @ViewChild('frame', { static: false }) frame;
  @Output() installationCreated: EventEmitter<void> = new EventEmitter<void>();
  customers = [];
  selectedCustomerId: number;
  typeahead = new EventEmitter<string>();

  constructor(
    private installationsService: InstallationsService,
    private customersPnService: CustomersPnService,
    private cd: ChangeDetectorRef
  ) {
    this.typeahead
      .pipe(
        debounceTime(200),
        switchMap((term) => {
          const model = new CustomersPnRequestModel();
          model.name = term;
          return this.customersPnService.getAllCustomers(model);
        })
      )
      .subscribe((items) => {
        const customers = [];
        for (const customer of items.model.customers) {
          customers.push({
            id: customer.id,
            label: this.getCustomerLabel(customer),
          });
        }
        this.customers = customers;
        this.cd.markForCheck();
      });
  }

  ngOnInit() {}

  show() {
    this.frame.show();
  }

  createInstallation() {
    this.installationsService
      .create(this.selectedCustomerId)
      .subscribe((data) => {
        if (data && data.success) {
          this.frame.hide();
          this.installationCreated.emit();
        }
      });
  }

  getCustomerLabel(customer: CustomerPnModel): string {
    const companyName = customer.companyName;
    const companyAddress = customer.companyAddress;
    const companyAddress2 = customer.companyAddress2;
    const zipCode = customer.zipCode;
    const cityName = customer.cityName;
    const countryCode = customer.countryCode;

    return (
      (companyName ? companyName + ' - ' : '') +
      (companyAddress ? companyAddress + ' - ' : '') +
      (companyAddress2 ? companyAddress2 + ' - ' : '') +
      (zipCode ? zipCode + ' - ' : '') +
      (cityName ? cityName + ' - ' : '') +
      (countryCode ? countryCode + ' - ' : '')
    );
  }
}
