import { Component } from '@angular/core';
import { UserService } from './shared/services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private userService: UserService, private router: Router)
  {
  }

  title = 'app';

  isLoggedIn() : boolean {
    return this.userService.isAuthenticated();
  }

  isNotLoggedIn() : boolean {
    return !this.userService.isAuthenticated();
  }

  logout() {
    this.userService.logout();
    this.router.navigate(['/']);    
  } 
}
