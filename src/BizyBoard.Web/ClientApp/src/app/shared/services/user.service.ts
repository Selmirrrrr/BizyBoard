import { Injectable, Inject } from '@angular/core';
import { Http , Response, Headers, RequestOptions } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';

import { UserRegistration } from '../models/user.registration.interface';

import { BaseService } from "./base.service";

import { Observable } from 'rxjs';
import { BehaviorSubject } from 'rxjs'; 

// Add the RxJS Observable operators we need in this app.
import { map } from 'rxjs/operators';
import { catchError } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable()

export class UserService extends BaseService {

  baseUrl: string = '';

  // Observable navItem source
  private _authNavStatusSource = new BehaviorSubject<boolean>(false);
  // Observable navItem stream
  authNavStatus$ = this._authNavStatusSource.asObservable();

  private loggedIn = false;

  constructor(private http: Http, @Inject('BASE_URL') baseUrl: string, public jwtHelper: JwtHelperService) {
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

    register(email: string, password: string, firstName: string, lastName: string, winBiuUsername: string, winBizPassword: string, company: string): Observable<UserRegistration> {
    let body = JSON.stringify({ email, password, firstName, lastName, winBiuUsername, winBizPassword, company });
    let headers = new Headers({ 'Content-Type': 'application/json' });
    let options = new RequestOptions({ headers: headers });

    return this.http.post(this.baseUrl + "api/auth/register", body, options).pipe(
      map(res => true), catchError(this.handleError)).source;
  }  

   login(email, password) {
    let headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this.http
      .post(
        this.baseUrl + 'api/auth/login',
        JSON.stringify({ email, password }),{ headers }
      )
      .pipe(map(res => res.json()),
      map(res => {
        localStorage.setItem('auth_token', res.auth_token);
        this.loggedIn = true;
        this._authNavStatusSource.next(true);
        return true;
      }),
      catchError(this.handleError)).source;
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