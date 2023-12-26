import { Message } from './../../_models/message';
import { CommonModule } from '@angular/common';
import { MessageService } from './../../_services/message.service';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, TimeagoModule, FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  messageContent = '';
  loading = false;

  constructor(public messageService: MessageService) { }

  ngOnInit(): void {
  }

  sendMessage() {
    if(!this.username) return;
    this.loading = true;
    // the sendMessage() method returns the promise -> use 'then' to act
    this.messageService.sendMessage(this.username, this.messageContent)
      .then(() => {
        this.messageForm?.reset();
      })
      .finally(() => this.loading = false)
  }
}
