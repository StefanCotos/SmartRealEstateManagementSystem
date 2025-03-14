import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(private readonly fb: FormBuilder, private readonly userService: UserService,
    private readonly router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.loginForm.reset();
  }

  onSubmit(): void {
    if (this.loginForm.valid) {

      this.userService.login(this.loginForm.value).subscribe({
        next: () => {
          if (this.userService.isAdmin()) {
            this.router.navigate(['admin']);
          } else {
            this.router.navigate(['']);
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

  navigateToRegister(): void {
    this.router.navigate(['register']);
  }

  navigateToForgotPassword(): void {
    this.router.navigate(['forgot-password']);
  }
}