import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControlOptions } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PasswordMatchValidator } from '../register/password-match.validator';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent {

  resetPasswordForm: FormGroup;
  message: string = '';
  errorMessage: string = '';
  token: string = '';

  constructor(private readonly fb: FormBuilder, private readonly userService: UserService, private readonly router: Router, private readonly route: ActivatedRoute) {

    this.resetPasswordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]]
    }, {
      validator: PasswordMatchValidator('password', 'confirmPassword')
    } as AbstractControlOptions);
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      if (!this.token) {
        this.errorMessage = 'Invalid or missing token';
      }
    });
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid && this.token) {
      const formValue = this.resetPasswordForm.value;

      this.userService.requestPasswordReset(this.token, formValue.password).subscribe({
        next: (data) => {
          this.message = data.data;

          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.errorMessage = err.error;
        }
      });
    }
    else {
      this.errorMessage = 'Invalid or missing token';
    }
    setTimeout(() => {
      this.errorMessage = '';
    }, 5000);
  }
}
