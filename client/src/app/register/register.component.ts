import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // it like: the registerCom want to get some infomation from HomeCom
  // @Input() usersFromHomeComponent: any;
  // to go from a child component to a parent
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({}); // declare the reactive form
  maxDate: Date = new Date(); // get the present day -> mean right now
  validationErrors: string[] | undefined;

  constructor(private accountService: AccountService, private toastr: ToastrService, 
    private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      // if we does not satisfy the Validator -> will have the error(error - object of this control)
      username: ['', Validators.required], // formControl means the fields/controls in the form
      gender: ['male'],
      knowAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });
    // make sure the form still valid when the password changes, it requires the confirmPassword changes as well
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: ()=> this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  // create a custom validator using 'ValidatorFn'
  matchValues(matchTo: string) : ValidatorFn {
    return (control: AbstractControl) => {
      // this control is represent for confirmPassword control and 
      // control.parent?.get(matchTo) is represent for password control
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true} // null is valid
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob};// flat each value from registerForm to values, and set the dateOfBirth with the given value dob 
    this.accountService.register(values).subscribe({
      next: () => {
        this.router.navigateByUrl('/members')
      },
      error: error => {
        this.validationErrors = error // this is array we do in error.interceptor
      }
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined) {
    if(!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10)
  }

}
