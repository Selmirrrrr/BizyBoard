import { OnInit, OnDestroy, Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { UpdatePwdModel } from '../../../shared/models/credentials.interface';
import { UserService } from '../../../shared/services/user.service';
import { Router, ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'update-pwd.component.html'
})
export class UpdatePwdComponent implements OnInit, OnDestroy {

  private subscription: Subscription;

  code: boolean;
  errors: string[] = [];
  isRequesting: boolean;
  submitted = false;
  credentials: UpdatePwdModel = { email: '', newPassword: '', passwordConfirmation: '', token: '' };

  constructor(private userService: UserService, private router: Router, private activatedRoute: ActivatedRoute) { }

    ngOnInit() {

    // subscribe to router event
    this.subscription = this.activatedRoute.queryParams.subscribe(
      (param: any) => {
         this.credentials.token = param['token'];
         this.credentials.email = param['email'];
      });
  }

   ngOnDestroy() {
    // prevent memory leak by unsubscribing
    this.subscription.unsubscribe();
  }

  updatePassword({ value, valid }: { value: UpdatePwdModel, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = [];
    if (valid) {
      this.userService.updatePassword(value.email, value.newPassword, value.passwordConfirmation, value.token)
        .pipe(finalize(() => this.isRequesting = false))
        .subscribe(
        result => {
          console.log(result);
          if (result) {
            this.router.navigate(['/login'], { queryParams: { email: value.email } });
          }
        },
        errors => {
          // this.successfulSave = false;
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
    this.isRequesting = false;
  }
}

