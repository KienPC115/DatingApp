import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

// ng add ngx-bootstrap -> to add the bootstrap on project
// npm i font-awesome -> to use font-awesome on project
//  mkcert -install  -> adding HTTPS to Angular using mkcert -> but on Windows u need to use the server.crt file

// Observables:
// - lazy collections of multiple values overtime
// - only subscribers of newsletter receive the newsletter
// - if no-one subscribes to the newsletter it probably will be printed
// 3 method: .subscribe(), .pipe() (angular automatically to sub or unsub the obs), .toPromise()


// Persisting the login => store the info into local Storage => when browser refreshes => we can take a peek inside this area


export class AppComponent implements OnInit {
  title = 'The Dating app';
  users: any;

  constructor(private accountService: AccountService, private presence: PresenceService) {}

  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser() {
    // here should to get user in the localStorage, because it represent the app and run first with the other component
    const userString = localStorage.getItem('user');
    if(!userString) return; // to check userString null or not -> because the JSON.parse() => can not parse the null

    const user : User = JSON.parse(userString);

    this.accountService.setCurrentUser(user);
    this.presence.createHubConnection(user);
  }

}
