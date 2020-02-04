import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {InstallationCasePageComponent} from './components';

const routes: Routes = [
  {path: ':id/:templateId/:installationId', component: InstallationCasePageComponent}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InstallationCaseRoutingModule { }
