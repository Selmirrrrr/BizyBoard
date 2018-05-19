import { Dossier } from './../models/credentials.interface';
import { Injectable, Inject } from '@angular/core';
import { Response, Headers, RequestOptions } from '@angular/http';
import { HttpClientModule, HttpClient } from '@angular/common/http';

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

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, public jwtHelper: JwtHelperService) {
    super();
    this.baseUrl = baseUrl;
  }

    testWinBizCredentials(email: string,
      password: string,
      firstName: string,
      lastName: string,
      winBizUsername: string,
      winBizPassword: string,
      company: string): Observable<Dossier[]> {
    const body = JSON.stringify({ email, password, firstName, lastName, winBizUsername, winBizPassword, company });

    return this.http.post<Dossier[]>(this.baseUrl + 'api/auth/testWinBizCredentials', body).pipe(
      map(res => res, catchError(this.handleError)));
  }
  getDocInfoVenteChiffreAffaire() {
    let result: any;
    this.http.get(this.baseUrl + 'api/board/GetDocInfoVenteChiffreAffaire').subscribe(r => {
      result = r;
    }, error => result = error);
    return result;
  }
}
