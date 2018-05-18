import { UpdatePwdComponent } from './views/auth/update-pwd/update-pwd.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { HttpModule } from '@angular/http';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};

import { AlertModule } from 'ngx-bootstrap/alert';

import { AuthenticateXHRBackend } from './authenticate-xhr.backend';
import { RouterModule } from '@angular/router';

// Import containers
import { DefaultLayoutComponent } from './containers';

import { AppComponent } from './app.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/auth/login/login.component';
import { RegisterComponent } from './views/auth/register/register.component';
import { ResetPwdComponent } from './views/auth/reset-pwd/reset-pwd.component';

const APP_CONTAINERS = [
  DefaultLayoutComponent
];

import { UserService } from './shared/services/user.service';
import { DataService } from './shared/services/data.service';
import { AuthGuard, AuthGuardLogin } from './auth.guard';

import { JwtModule } from '@auth0/angular-jwt';
import { AppRoutingModule } from './app.routing';

// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ChartsModule } from 'ng2-charts/ng2-charts';

import {
  AppAsideModule,
  AppBreadcrumbModule,
  AppHeaderModule,
  AppFooterModule,
  AppSidebarModule,
} from '@coreui/angular';

export function tokenGetter() {
  return localStorage.getItem('auth_token');
}

@NgModule({
  declarations: [
    AppComponent,
    ...APP_CONTAINERS,
    P404Component,
    P500Component,
    LoginComponent,
    RegisterComponent,
    FetchDataComponent,
    ResetPwdComponent,
    UpdatePwdComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserModule,
    AppRoutingModule,
    AppAsideModule,
    AppBreadcrumbModule.forRoot(),
    AppFooterModule,
    AppHeaderModule,
    AppSidebarModule,
    PerfectScrollbarModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    AlertModule.forRoot(),
    ChartsModule,
    HttpClientModule,
    FormsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ['localhost:3001, localhost:52003'],
        blacklistedRoutes: ['localhost:3001/login/']
      }
    }),
    HttpModule
  ],
  providers: [{
    provide: LocationStrategy,
    useClass: HashLocationStrategy
  }, UserService, DataService, AuthGuard, AuthGuardLogin],
  bootstrap: [AppComponent]
})
export class AppModule { }
