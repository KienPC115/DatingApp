import { AccountService } from './../_services/account.service';
import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  //ng g interceptor _interceptors/jwt --skip-tests          
  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // get the token in localStorage and add into the Headers
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user) {
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })

    return next.handle(request);
  }
}
