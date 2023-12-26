import { ToastrService } from 'ngx-toastr';
import { AdminService } from 'src/app/_services/admin.service';
import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit{
  photos: Photo[] = []

  constructor(private adminService: AdminService, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.getPhotosForApproval()
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => this.photos = photos
    })
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: _ => {
        this.photos.splice(this.photos.findIndex(x => x.id === photoId), 1)
        this.toastr.success('Approve success this photo')
      }
    })
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: _ => {
        this.photos.splice(this.photos.findIndex(x => x.id === photoId), 1)
        this.toastr.info('Reject success this photo')
      }
    })
  }
}
