import { Dossier } from './../models/credentials.interface';
import { Injectable, Inject } from '@angular/core';
import { Http , Response, Headers, RequestOptions } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';

import { UserRegistration } from '../models/user.registration.interface';

import { BaseService } from './base.service';

import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

// Add the RxJS Observable operators we need in this app.
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable()

export class DataService extends BaseService {

  baseUrl = '';

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string, public jwtHelper: JwtHelperService) {
    super();
    this.baseUrl = baseUrl;
  }

  public isAuthenticated(): boolean {
    const token = localStorage.getItem('auth_token');
    // Check whether the token is expired and return
    // true or false
    return !this.jwtHelper.isTokenExpired(token);
  }

    testWinBizCredentials(email: string,
      password: string,
      firstName: string,
      lastName: string,
      winBizUsername: string,
      winBizPassword: string,
      company: string): Observable<Dossier[]> {
    const body = JSON.stringify({ email, password, firstName, lastName, winBizUsername, winBizPassword, company });
    const headers = new Headers({ 'Content-Type': 'application/json' });
    const options = new RequestOptions({ headers: headers });

    return this.http.post(this.baseUrl + 'api/auth/testWinBizCredentials', body, options).pipe(
      map(res => res.json()), catchError(this.handleError)).source;
  }
}
