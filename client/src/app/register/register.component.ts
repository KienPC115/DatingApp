import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

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

  model: any = {};

  constructor(private accountService: AccountService ) { }

  ngOnInit(): void {

  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: error => console.log(error)
    })
  
  }
  
  cancel() {
    this.cancelRegister.emit(false);
  }


}
