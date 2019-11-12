import {ChangeDetectorRef, Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {debounceTime, switchMap} from 'rxjs/operators';
import {CustomerPnModel, CustomersPnRequestModel} from '../../../customers-pn/models/customer';
import {InstallationsService} from '../../services';

@Component({
  selector: 'app-installation-new',
  templateUrl: './installation-new.component.html',
  styleUrls: ['./installation-new.component.scss']
})
export class InstallationNewComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() installationCreated: EventEmitter<void> = new EventEmitter<void>();
  customersRequestModel: CustomersPnRequestModel = new CustomersPnRequestModel();
  customers = [];
  selectedCustomerId: number;
  typeahead = new EventEmitter<string>();

  spinnerStatus = false;

  constructor(
    private installationsService: InstallationsService,
    private cd: ChangeDetectorRef
  ) {
    this.typeahead.pipe(
        debounceTime(200),
        switchMap(term => {
          this.customersRequestModel.name = term;
          return this.installationsService.getAllCustomers(this.customersRequestModel);
        })
      ).subscribe(items => {
        const customers = [];
        for (const customer of items.model.customers) {
          customers.push({
            id: customer.id,
            label: this.getCustomerLabel(customer)
          });
        }
        this.customers = customers;
        this.cd.markForCheck();
      });
  }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  createInstallation() {
    this.spinnerStatus = true;

    this.installationsService.create(this.selectedCustomerId).subscribe((data) => {
      if (data && data.success) {
        this.frame.hide();
        this.installationCreated.emit();
      }
      this.spinnerStatus = false;
    });
  }

  getCustomerLabel(customer: CustomerPnModel): string {
      const companyName = customer.fields.find(f => f.name === 'CompanyName');
      const companyAddress = customer.fields.find(f => f.name === 'CompanyAddress');
      const companyAddress2 = customer.fields.find(f => f.name === 'CompanyAddress2');
      const zipCode = customer.fields.find(f => f.name === 'ZipCode');
      const cityName = customer.fields.find(f => f.name === 'CityName');
      const countryCode = customer.fields.find(f => f.name === 'CountryCode');

      return (companyName ? companyName.value + ' - ' : '') +
        (companyAddress ? companyAddress.value + ' - ' : '') +
        (companyAddress2 ? companyAddress2.value + ' - ' : '') +
        (zipCode ? zipCode.value + ' - ' : '') +
        (cityName ? cityName.value + ' - ' : '') +
        (countryCode ? countryCode.value + ' - ' : '');
  }
}
