import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControlOptions } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { PasswordMatchValidator } from '../register/password-match.validator';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-user-change-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './user-change-password.component.html',
  styleUrl: './user-change-password.component.css'
})
export class UserChangePasswordComponent implements OnInit {
  changeForm: FormGroup;
  errorMessage: string = '';
  user: any;

  constructor(private readonly fb: FormBuilder, private readonly userService: UserService,
    private readonly router: Router) {
    this.changeForm = this.fb.group({
      currentPassword: ['', [Validators.required, Validators.minLength(6)]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
    }, {
      validator: PasswordMatchValidator('newPassword', 'confirmPassword')
    } as AbstractControlOptions);
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.changeForm.reset();
    const userId = this.userService.getUserId();
    this.userService.getUserById(userId).subscribe(data => {
      this.user = data;
    });
  }

  onSubmit(): void {
    if (this.changeForm.valid) {
      this.user.password = this.changeForm.value.currentPassword;
      this.userService.checkPassword(this.user).subscribe({
        next: (response: boolean) => {
          if (response) {
            this.user.password = this.changeForm.value.newPassword;
            this.userService.changePassword(this.user).subscribe({
              next: () => {
                this.router.navigate(['profile']);
              },
              error: (err) => {
                this.errorMessage = err.error;
              }
            });
          }
        },
        error: (err) => {
          this.errorMessage = err.error;
        }
      });
    }
    setTimeout(() => {
      this.errorMessage = '';
    }, 5000);
  }

  navigateToProfile(): void {
    this.router.navigate(['profile']);
  }

}
