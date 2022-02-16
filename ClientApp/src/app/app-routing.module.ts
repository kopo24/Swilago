import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { BrowserUtils } from '@azure/msal-browser';
import { RecordComponent } from './record/record.component';
import { HomeComponent } from './home/home.component';
import { FailedComponent } from './failed/failed.component';
import { EnrollmentComponent } from './enrollment/enrollment.component';

const routes: Routes = [
  {
    path: 'record',
    component: RecordComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'enrollment',
    component: EnrollmentComponent,
    canActivate: [MsalGuard]
  },
  {
    path: '',
    component: HomeComponent,
    children: []
  },
  {
    path: '**',
    component: FailedComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    initialNavigation: !BrowserUtils.isInIframe() && !BrowserUtils.isInPopup() ? 'enabled' : 'disabled'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
