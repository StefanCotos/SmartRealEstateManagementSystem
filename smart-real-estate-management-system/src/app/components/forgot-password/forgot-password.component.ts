import { Component } from '@angular/core';
import { NavbarComponent } from "../navbar/navbar.component";
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css'
})
export class ForgotPasswordComponent {

  forgotPasswordForm: FormGroup;
  message: string = '';
  errorMessage: string = '';


  constructor(private readonly fb: FormBuilder, private readonly userService: UserService, private readonly router: Router) {

    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });


  }

  onSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      const formValue = this.forgotPasswordForm.value;
      this.userService.requestSendLink(formValue.email).subscribe({
        next: (data) => {
          this.message = data.data
        },
        error: (err) => {
          this.errorMessage = err.error;
        }
      }
      );
    }

    setTimeout(() => {
      this.errorMessage = '';
    }, 5000);
  }


}

