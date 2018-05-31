import { LabelValue } from './data.service';
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

    return this.http.post<Dossier[]>(this.baseUrl + 'api/auth/testWinBizCredentials', {
      email, password, firstName, lastName, winBizUsername, winBizPassword, company
    }).pipe(
      map(res => res, catchError(this.handleError)));
  }

  public getDocInfoVenteChiffreAffaireYears(nbYears: number): Observable<LabelValue[]> {
    return this.http.get<LabelValue[]>(this.baseUrl + 'api/board/GetDocInfoVenteChiffreAffaireYears/' + nbYears);
  }

  public getDocInfoVenteChiffreAffaireMonths(nbMonths: number): Observable<LabelValue[]> {
    return this.http.get<LabelValue[]>(this.baseUrl + 'api/board/GetDocInfoVenteChiffreAffaireMonths/' + nbMonths);
  }

  public getSalesThisAndPastYearMonth(): any {
    return this.http.get(this.baseUrl + 'api/board/GetSalesThisAndPastYearMonth');
  }

  public getSalesThisAndPastYear(): any {
    return this.http.get(this.baseUrl + 'api/board/GetSalesThisAndPastYear');
  }

  public GetPaymentsCalendar(): any {
    return this.http.get(this.baseUrl + 'api/board/GetPaymentsCalendar');
  }

  public getPendingPayments(): any {
    return this.http.get(this.baseUrl + 'api/board/GetPendingPayments');
  }
}

export interface LabelValue {
  label: string;
  value: number;
}
