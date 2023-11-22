import { ResolveFn } from '@angular/router';
import { Member } from '../_models/member';
import { inject } from '@angular/core';
import { MembersService } from '../_services/members.service';
// ng g r _resolvers/member-detailed --skip-tests -> to create this class resolver
// this problem is we can not select tab by get tabName(Messages) in the params before the view tab initial and load the member so that we can not select the tab
// it help we load and hold the member immediately before when the component is constructed
export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MembersService);

  return memberService.getMember(route.paramMap.get('username')!)
};
