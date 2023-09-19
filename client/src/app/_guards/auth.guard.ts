import { inject } from '@angular/core';
import { AccountService } from './../_services/account.service';
import { CanActivateFn } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

// Angular route guard to protect the route can not access without authentication(they are not allow to get to route)
// cli: ng g guard [Name], then choose canActivaten

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  
  return accountService.currentUser$.pipe(
    map(user => {
      if(user) return true;
      else{
        toastr.error('you shall not pass!');
        return false;
      }
    })
  );
};
