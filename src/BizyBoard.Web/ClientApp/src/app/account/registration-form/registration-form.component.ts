
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { UserRegistration } from '../../shared/models/user.registration.interface';
import { UserService } from '../../shared/services/user.service';

import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-registration-form',
  templateUrl: './registration-form.component.html',
  styleUrls: ['./registration-form.component.css']
})

export class RegistrationFormComponent implements OnInit {
  errors: string[] = [];
  isRequesting: boolean;
  submitted: boolean = false;

  constructor(private userService: UserService,private router: Router) { }

  ngOnInit() {
  }

  registerUser({ value, valid }: { value: UserRegistration, valid: boolean }) {
    this.submitted = true;
    this.isRequesting = true;
    this.errors= [];
    if(valid)
    {
      this.userService.register(value.email,value.password,value.firstName,value.lastName, value.winBizUsername, value.winBizPassword, value.company).pipe(
                  finalize(() => this.isRequesting = false)
                )
                .subscribe(
                  result  => {if(result){
                      this.router.navigate(['/login'],{queryParams: {brandNew: true,email:value.email}});                         
                  }},
                  errors => {
                    // this.successfulSave = false;
                    if (errors.status === 400) {
                        // handle validation error
                        let validationErrorDictionary = JSON.parse(errors.text());
                        for (var fieldName in validationErrorDictionary) {
                            if (validationErrorDictionary.hasOwnProperty(fieldName)) {
                              this.errors.push(validationErrorDictionary[fieldName]);
                            }
                        }
                    } else {
                        this.errors.push("something went wrong!");
                    }});
    }      
  } 
}