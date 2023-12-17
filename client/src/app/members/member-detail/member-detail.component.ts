import { AccountService } from './../../_services/account.service';
import { PresenceService } from './../../_services/presence.service';
import { MessageService } from './../../_services/message.service';
import { MembersService } from 'src/app/_services/members.service';
import { Member } from './../../_models/member';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs';

// npm i ng-gallery @angular/cdk -> to use galleryModule
// -> integrate 3rd party component into application
@Component({
  selector: 'app-member-detail',
  standalone: true, // be come a standalone component -> it can flexibility in order to use the module they want -> ex: here we use the GalleryModule to manage to the photos of each user
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  constructor(private accountService: AccountService, private route: ActivatedRoute, 
    private messageService: MessageService, public presenceService: PresenceService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe({
          next: user => {
            if(user) this.user = user;
          }
        })
     }

  ngOnInit(): void {
    // when our component is initialized and ngOnInit happens before our view is initialized
    // here this will get the data by resolver - member-detailed.resolver
    this.route.data.subscribe({
      next: data => this.member = data['member']
    });

    // on selected tab when this component initialize
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages();
  }
   
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === "Messages" && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    }
    else {
      // chuyển qua tab khác sẽ ngắt kết nối với MessageHub
      this.messageService.stopHubConnection();
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      });
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }

}
