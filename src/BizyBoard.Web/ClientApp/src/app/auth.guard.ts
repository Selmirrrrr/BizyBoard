import { Injectable } from '@angular/core';
import { Router, CanActivate, CanDeactivate } from '@angular/router';
import { UserService } from './shared/services/user.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private userService: UserService, private router: Router) {}

  canActivate(): boolean {
    if (!this.userService.isAuthenticated()) {
      this.router.navigate(['login']);
      return false;
    }
    return true;
  }
}

@Injectable()
export class AuthGuardLogin implements CanActivate {
  constructor(private userService: UserService, private router: Router) {}

  canActivate(): boolean {
    return !this.userService.isAuthenticated();
  }
}