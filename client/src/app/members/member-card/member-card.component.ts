import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { Member } from 'src/app/_models/member';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})

//ng g c members/member-card --skip-tests -> to create this component

export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined;

  constructor() {}
 
  ngOnInit(): void {
  }





}
