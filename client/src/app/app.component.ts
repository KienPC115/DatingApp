import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

// ng add ngx-bootstrap -> to add the bootstrap on project
// npm i font-awesome -> to use font-awesome on project
//  mkcert -install  -> adding HTTPS to Angular using mkcert -> but on Windows u need to use the server.crt file

export class AppComponent implements OnInit {
  
  title = 'Dating app';
  users: any;

  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    }); // .subcribe to observe an observable
  }


}
