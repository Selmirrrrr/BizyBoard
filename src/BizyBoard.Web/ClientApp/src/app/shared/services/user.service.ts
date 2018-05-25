import { Injectable, Inject } from '@angular/core';
import { Response, Headers, RequestOptions } from '@angular/http';
import { HttpClientModule, HttpHeaders } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';


import { UserRegistration } from '../models/user.registration.interface';

import { BaseService } from './base.service';

import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

// Add the RxJS Observable operators we need in this app.
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable()

export class UserService extends BaseService {

  baseUrl = '';

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  private loggedIn = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, public jwtHelper: JwtHelperService) {
    super();
    this.loggedIn = this.isAuthenticated();
    // ?? not sure if this the best way to broadcast the status but seems to resolve issue on page refresh where auth status is lost in
    // header component resulting in authed user nav links disappearing despite the fact user is still logged in
    this._authNavStatusSource.next(this.loggedIn);
    this.baseUrl = baseUrl;
  }
  public isAuthenticated(): boolean {
    const token = localStorage.getItem('auth_token');
    // Check whether the token is expired and return
    // true or false
    return !this.jwtHelper.isTokenExpired(token);
  }

    register(email: string,
      password: string,
      firstName: string,
      lastName: string,
      winBizUsername: string,
      winBizPassword: string,
      company: string,
      dossier: number,
      year: number): Observable<UserRegistration> {
    return this.http.post(this.baseUrl + 'api/auth/register', {
      email, password, firstName, lastName, winBizUsername, winBizPassword, company, dossier, year }).pipe(
      map(res => true), catchError(this.handleError)).source;
  }

   login(email, password) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this.http
      .post<any>(
        this.baseUrl + 'api/auth/login',
        { email, password },
      )
      .pipe(map(res => res),
      map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      }),
      catchError(this.handleError)).source;
  }

  resetPassword(email: string) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');
    return this.http
      .post(this.baseUrl + 'api/auth/forgotpassword', JSON.stringify({email}))
      .pipe(map(res => true), catchError(this.handleError)).source;
  }

  updatePassword(email: string, newPassword: string, passwordConfirmation: string, token: string) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');
    return this.http
      .post(this.baseUrl + 'api/auth/updatepassword', JSON.stringify({email, newPassword, passwordConfirmation, token}))
      .pipe(map(res => true), catchError(this.handleError)).source;
  }

  logout() {
    localStorage.removeItem('auth_token');
    this.loggedIn = false;
    this._authNavStatusSource.next(false);
  }

  isLoggedIn() {
    return this.loggedIn;
  }
}
