import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from '../../../shared/services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { UserRegistration } from '../../../shared/models/user.registration.interface';
import { Credentials } from '../../../shared/models/credentials.interface';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'login.component.html'
})
export class LoginComponent implements OnInit, OnDestroy {

  private subscription: Subscription;
  brandNew: boolean;
  errors: string[] = [];
  public isRequesting: boolean;
  submitted = false;
  credentials: Credentials = { email: '', password: '' };

  options = {
    ease: 'linear',
    speed: 200,
    trickleSpeed: 300,
    meteor: true,
    spinner: true,
    spinnerPosition: 'right',
    direction: 'leftToRightIncreased',
    color: 'red',
    thick: true
  };

  startedClass = false;
  completedClass = false;
  preventAbuse = false;

  constructor(private userService: UserService, private router: Router, private activatedRoute: ActivatedRoute) { }

    ngOnInit() {

    // subscribe to router event
    this.subscription = this.activatedRoute.queryParams.subscribe(
      (param: any) => {
         this.brandNew = param['brandNew'];
         this.credentials.email = param['email'];
      });
  }

   ngOnDestroy() {
    // prevent memory leak by unsubscribing
    this.subscription.unsubscribe();
  }

  login({ value, valid }: { value: Credentials, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = [];
    if (valid) {
      this.userService.login(value.email, value.password)
        .subscribe(
        result => {
          this.isRequesting = false;
          if (result) {
             this.router.navigate(['/dashboard']);
          }
        },
        errors => {
          this.isRequesting = false;
          if (errors.status === 400) {
              // handle validation error
              const validationErrorDictionary = errors.error;
              for (const fieldName in validationErrorDictionary) {
                  if (validationErrorDictionary.hasOwnProperty(fieldName)) {
                    this.errors.push(validationErrorDictionary[fieldName]);
                  }
              }
          } else {
            this.errors.push(errors.error);
          }});
    }
  }
}

