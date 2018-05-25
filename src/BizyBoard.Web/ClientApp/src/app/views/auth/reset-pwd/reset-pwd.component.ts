import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from '../../../shared/services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Credentials, Email } from '../../../shared/models/credentials.interface';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'reset-pwd.component.html'
})
export class ResetPwdComponent implements OnInit, OnDestroy {
  message: string;
  email: Email = { email: ''};
  errors: string[] = [];
  isRequesting: boolean;
  submitted = false;

  constructor(private userService: UserService, private router: Router, private activatedRoute: ActivatedRoute) { }

    ngOnInit() {

    }

    ngOnDestroy() {

    }

  resetPassword({ value, valid }: { value: Email, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = [];
    if (valid) {
      this.userService.resetPassword(value.email)
        .pipe(finalize(() => this.isRequesting = false))
        .subscribe(
        result => {
          this.message = 'Vous avez reçu un email avec les instructions à suivre...';
        },
        errors => {
          // this.successfulSave = false;
          if (errors.status === 400) {
              // handle validation error
              const validationErrorDictionary = JSON.parse(errors.text());
              for (const fieldName in validationErrorDictionary) {
                  if (validationErrorDictionary.hasOwnProperty(fieldName)) {
                    this.errors.push(validationErrorDictionary[fieldName]);
                  }
              }
          } else {
            this.errors.push(errors.text());
          }});
    }
    this.isRequesting = false;
  }
}

