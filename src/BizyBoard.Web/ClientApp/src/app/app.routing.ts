import { UpdatePwdComponent } from './views/auth/update-pwd/update-pwd.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import { DefaultLayoutComponent } from './containers';

import { AuthGuard, AuthGuardLogin } from './auth.guard';
import { FetchDataComponent } from './fetch-data/fetch-data.component';

import { P404Component } from './views/error/404.component';
import { P500Component } from './views/error/500.component';
import { LoginComponent } from './views/auth/login/login.component';
import { RegisterComponent } from './views/auth/register/register.component';
import { ResetPwdComponent } from './views/auth/reset-pwd/reset-pwd.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '',
    pathMatch: 'full',
  },
  {
    path: '404',
    component: P404Component,
    data: {
      title: 'Page 404'
    }
  },
  {
    path: '500',
    component: P500Component,
    data: {
      title: 'Page 500'
    }
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [AuthGuardLogin],
    data: {
      title: 'Login Page'
    }
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [AuthGuardLogin],
    data: {
      title: 'Register Page'
    }
  },
  {
    path: 'resetpwd',
    component: ResetPwdComponent,
    canActivate: [AuthGuardLogin],
    data: {
      title: 'Reset password page'
    }
  },
  {
    path: 'updatepwd',
    component: UpdatePwdComponent,
    canActivate: [AuthGuardLogin],
    data: {
      title: 'Update password page'
    }
  },
  {
    path: '',
    component: DefaultLayoutComponent,
    data: {
      title: 'Home'
    },
    children: [
      {
        path: 'dashboard',
        loadChildren: './views/dashboard/dashboard.module#DashboardModule',
        canActivate: [AuthGuard]
      },
    ]
  }
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}
