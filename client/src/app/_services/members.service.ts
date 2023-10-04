import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // using get member with the token(in headers) because in backend has Authentication in this method
  // jwt interceptor will help us add token into the headers of request
  getMembers() {
    return this.http.get<Member[]>(this.baseUrl+'users')
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + username)
  }

  
}
