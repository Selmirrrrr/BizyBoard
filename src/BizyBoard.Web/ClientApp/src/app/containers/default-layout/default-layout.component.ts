import { Component, Input } from '@angular/core';
import { UserService } from '../../shared/services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent {
  public sidebarMinimized = true;
  private changes: MutationObserver;
  public element: HTMLElement = document.body;
  constructor(private userService: UserService, private router: Router) {

    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
    });

    this.changes.observe(<Element>this.element, {
      attributes: true
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
    this.router.navigate(['']);
  }
}
