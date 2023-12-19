import { ConfirmService } from './../_services/confirm.service';
import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';

// ng g g _guards/prevent-unsaved-changes --skip-tests
// choose Deactivate -> to create this guard

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService = inject(ConfirmService);
  
  // will notice for user when they are updating info. Want to confirm from user continue or not -> for go to another route
  if(component.editFormm?.dirty) {
    return confirmService.confirm();
  }
  
  return true;
};
