import { PresenceService } from './../../_services/presence.service';
import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})

//ng g c members/member-card --skip-tests -> to create this component

export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined;

  constructor(private memberService: MembersService, private toastr: ToastrService, 
    public presenceService : PresenceService) {}
 
  ngOnInit(): void {
  }

  addLike(member: Member) {
    this.memberService.addLike(member.userName).subscribe({
      next: () => this.toastr.success('You have liked ' + member.knowAs)
    })
  }



}
