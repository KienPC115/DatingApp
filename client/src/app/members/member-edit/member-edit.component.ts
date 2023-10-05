import { take } from 'rxjs';
import { AccountService } from './../../_services/account.service';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { MembersService } from 'src/app/_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})

// ng g c members/member-edit --skip-tests -> create this component
export class MemberEditComponent implements OnInit{
  // @ViewChild is the feature of Angular -> the templates is a child of our component (ex: the form is child of member-edit)
  @ViewChild('editForm') editFormm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any) {
    if(this.editFormm?.dirty) {
      // @event it represent event from the browser
      $event.returnValue = true;
    }
  }
  
  member: Member | undefined;
  user: User | null = null;

  constructor(private accountService: AccountService, private memberService:MembersService,
    private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if(!this.user) return;
    this.memberService.getMember(this.user.username).subscribe({
      next: member => this.member = member
    })
  }

  updateMember() {
    this.memberService.updateMember(this.editFormm?.value).subscribe({
      next: _ => {
        this.toastr.success('Profile updated successfully');
        this.editFormm?.reset(this.member);
      }
    })
  }

}
