import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {InstallationCheckingPnLayoutComponent} from './layouts';
import {AdminGuard, PermissionGuard} from '../../../common/guards';
import {InstallationCheckingSettingsComponent} from './components';
import {InstallationCheckingPnClaims} from './const';

export const routes: Routes = [
  {
    path: '',
    component: InstallationCheckingPnLayoutComponent,
    canActivate: [PermissionGuard],
    data: {requiredPermission: InstallationCheckingPnClaims.accessInstallationCheckingPlugin},
    children: [
      {
        path: 'settings',
        canActivate: [AdminGuard],
        component: InstallationCheckingSettingsComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InstallationCheckingPnRoutingModule {
}
