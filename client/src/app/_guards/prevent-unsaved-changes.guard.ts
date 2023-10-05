import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

// ng g g _guards/prevent-unsaved-changes --skip-tests
// choose Deactivate -> to create this guard

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  // will notice for user when they are updating info. Want to confirm from user continue or not -> for go to another route
  if(component.editFormm?.dirty) {
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
  }
  
  return true;
};
