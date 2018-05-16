import { Component, OnInit } from '@angular/core';
import { UserService } from './shared/services/user.service';
import { Router, NavigationEnd } from '@angular/router';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable } from 'rxjs';

@Component({
  // tslint:disable-next-line
  selector: 'body',
  template: '<router-outlet></router-outlet>'
})
export class AppComponent implements OnInit {
  isHandset: Observable<BreakpointState> = this.breakpointObserver.observe(Breakpoints.Handset);
  title = 'app';

  constructor(private userService: UserService, private router: Router, private breakpointObserver: BreakpointObserver) {
  }

  ngOnInit() {
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }


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
