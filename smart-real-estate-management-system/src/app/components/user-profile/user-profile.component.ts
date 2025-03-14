import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.css'
})
export class UserProfileComponent implements OnInit {
  user: any;

  constructor(private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly userService: UserService) {
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.userService.getUserById(this.userService.getUserId()).subscribe(data => {
      this.user = data;
    });
  }

  removeProfile(): void {
    const password = prompt('Please enter your password to confirm:');
    if (!password) {
      alert('Password is required to remove the profile.');
      return;
    }
    else {
      this.user.password = password;
    }

    this.userService.checkPassword(this.user).subscribe({
      next: (response: boolean) => {
        if (response) {
          if (confirm('Are you sure you want to remove your profile and all data?\nThe action is irreversible.')) {
            this.userService.removeUser(this.userService.getUserId()).subscribe(() => {
              this.router.navigate(['']);
            });
          }
        }
      },
      error: (err) => {
        alert(err.error);
      }
    });
  }

  editProfile(): void {
    this.router.navigate(['users/edit']);
  }

  seeReviews(id: string): void {
    this.router.navigate(['review-users/get-all', id]);
  }

  seeEstates(): void {
    this.router.navigate(['users/estates-list']);
  }

  seeAcquisitions(): void {
    this.router.navigate(['users/acquisitions']);
  }

  changePassword(): void {
    this.router.navigate(['users/change-password']);
  }
}
