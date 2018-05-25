import { Dossier, Exercice } from './../../../shared/models/credentials.interface';
import { Component, OnInit, Output } from '@angular/core';
import { UserService } from '../../../shared/services/user.service';
import { DataService } from './../../../shared/services/data.service';
import { Router } from '@angular/router';
import { UserRegistration } from '../../../shared/models/user.registration.interface';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'register.component.html'
})
export class RegisterComponent implements OnInit {
  errors: string[] = [];
  dossiers: Dossier[] = [];
  isRequesting: boolean;
  submitted = false;
  selectedDossier: Dossier;
  selectedExercice: string;

  constructor(private userService: UserService, private dataService: DataService, private router: Router) { }

  ngOnInit() {
  }

  registerUser({ value, valid }: { value: UserRegistration, valid: boolean }) {
    if (this.dossiers.length < 1) { return this.testWinBizCredentials({ value, valid }); }
    this.submitted = true;
    this.isRequesting = true;
    this.errors = [];
    if (valid) {
      this.userService.register(value.email,
        value.password,
        value.firstName,
        value.lastName,
        value.winBizUsername,
        value.winBizPassword,
        value.company, JSON.parse(this.selectedExercice).dossier, JSON.parse(this.selectedExercice).year).pipe(
          finalize(() => this.isRequesting = false)
        )
        .subscribe(
          result => {
            if (result) {
              this.router.navigate(['/login'], { queryParams: { brandNew: true, email: value.email } });
            }
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
            }
          });
    }
  }

  testWinBizCredentials({ value, valid }: { value: UserRegistration, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors = [];
    if (valid) {
      this.dataService.testWinBizCredentials(value.email,
        value.password,
        value.firstName,
        value.lastName,
        value.winBizUsername,
        value.winBizPassword,
        value.company).pipe(
          finalize(() => this.isRequesting = false)
        )
        .subscribe(
          result => {
            if (result) {
              this.dossiers = result;
            }
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
            }
          });
    }
  }
}
