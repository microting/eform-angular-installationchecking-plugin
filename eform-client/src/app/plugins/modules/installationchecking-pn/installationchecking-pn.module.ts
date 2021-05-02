import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { NgSelectModule } from '@ng-select/ng-select';
import { SharedPnModule } from '../shared/shared-pn.module';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { EformSharedModule } from 'src/app/common/modules/eform-shared/eform-shared.module';
import { InstallationCheckingPnLayoutComponent } from './layouts';

import { InstallationCheckingPnRoutingModule } from './installationchecking-pn.routing.module';
import {
  InstallationCheckingPnSettingsService,
  InstallationsService,
} from './services';
import {
  InstallationAssignComponent,
  InstallationCheckingSettingsComponent,
  InstallationNewComponent,
  InstallationRetractComponent,
  InstallationsPageComponent,
  RemovalPageComponent,
} from './components';
import {
  installationPersistProvider,
  InstallationStateService,
} from './components/installation/store';
import {
  removalPersistProvider,
  RemovalStateService,
} from './components/removal/store';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    InstallationCheckingPnRoutingModule,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    EformSharedModule,
    FontAwesomeModule,
  ],
  declarations: [
    InstallationCheckingPnLayoutComponent,
    InstallationCheckingSettingsComponent,
    InstallationAssignComponent,
    InstallationNewComponent,
    InstallationRetractComponent,
    InstallationsPageComponent,
    RemovalPageComponent,
  ],
  providers: [
    InstallationCheckingPnSettingsService,
    InstallationsService,
    installationPersistProvider,
    InstallationStateService,
    removalPersistProvider,
    RemovalStateService,
  ],
})
export class InstallationCheckingPnModule {}
