import { Component } from '@angular/core';
import { UserService } from './shared/services/user.service';
import { Router } from '@angular/router';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  isHandset: Observable<BreakpointState> = this.breakpointObserver.observe(Breakpoints.Handset);

  constructor(private userService: UserService, private router: Router, private breakpointObserver: BreakpointObserver) {
  }

  title = 'app';

  isLoggedIn(): boolean {
    return this.userService.isAuthenticated();
  }

  isNotLoggedIn(): boolean {
    return !this.userService.isAuthenticated();
  }

  logout() {
    this.userService.logout();
    this.router.navigate(['/']);
  }
}
