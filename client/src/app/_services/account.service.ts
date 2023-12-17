import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})

// ng g s _services/account --skip-tests -> to add services to Angular

export class AccountService {

  baseUrl = environment.apiUrl;
  // create a currentUser here to all component can access it when use this service
  // inside service => so that we create it here
  private currentUserSource = new BehaviorSubject<User | null>(null); // special kind of observable for this called a behavior subject
  
  // used outside of the service - with '$' is a convention to signify this is unobservable
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: any) {
    // give httpclient a hint about what we return in a response
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User ) => {
        const user = response;
        if(user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl+'account/register',model).pipe(
      map(user => {
        if(user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  setCurrentUser(user : User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    // check roles is array or not, because the roles of user can be more than one.
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    // to store inside local storage
    localStorage.setItem('user', JSON.stringify(user)) // we should have access to that anywhere from our application
    this.currentUserSource.next(user);  // what its next value is and pass in that user
    // khi đăng nhập thì ng đó đã kết nối với Hub.
    this.presenceService.createHubConnection(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null); // set it to null if logout
    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token: string) {
    // parse the middle part of token - this contain the claims/roles of the user
    return JSON.parse(atob(token.split('.')[1]))
  }
}
