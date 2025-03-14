import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, NavbarComponent, ReactiveFormsModule, FooterComponent],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.css'
})
export class ContactComponent implements OnInit {

  contactForm: FormGroup;
  errorMessage: string = '';

  constructor(private readonly fb: FormBuilder, private readonly userService: UserService) {
    this.contactForm = this.fb.group({
      name: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      subject: ['', [Validators.required]],
      message: ['', [Validators.required]],

    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.contactForm.reset();
  }

  onSubmit(): void {
    if (this.contactForm.valid) {
      this.userService.sendContactForm(this.contactForm.value).subscribe({
        next: (data) => {
          this.contactForm.reset();
        },
        error: (err) => {
          this.errorMessage = err.error;
        }
      });
    }

  }
}
