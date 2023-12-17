import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  // npm install @microsoft/signalr to install SignalR into client-side
  private hubConnection?: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable(); // this gives us something to subscribe to from our components

  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect() // if it does lose a connection with our API server, is going to retry to connect to it. Client need to wait default 0->10 and 30s bef reconnect
      .build();

      this.hubConnection.start().catch(error => console.log(error));

      this.hubConnection.on('UserIsOnline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe({
          // the presence.service will exits all the time and contain the onlineUserSource it show for all user in application
          next: usernames => this.onlineUsersSource.next([...usernames, username])
        })
      })

      this.hubConnection.on('UserIsOffline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe({
          next: usernames => this.onlineUsersSource.next(usernames.filter(x => x !== username))
        })
      })

      this.hubConnection.on('GetOnlineUsers', usernames => {
        this.onlineUsersSource.next(usernames);
      })

      this.hubConnection.on('NewMessageReceived', ({username, knowAs}) => {
        // we add the tap event to toastr -> when we tap it will navigate us to the message tab with the sender message
        this.toastr.info(knowAs + ' has sent you a new message! Click me to see it')
          .onTap
          .pipe(take(1))
          .subscribe({
            next: () => this.router.navigateByUrl('/members/'+ username + '?tab=Messages')
          })
      })
    }

  stopHubConnection() {
    this.hubConnection?.stop().catch(error => console.log(error));
  }
}
