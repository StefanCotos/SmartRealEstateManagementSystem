import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { ReviewService } from '../../services/review.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-user-review',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './user-review.component.html',
  styleUrl: './user-review.component.css'
})
export class UserReviewComponent implements OnInit {
  reviewForm: FormGroup;

  constructor(private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly userService: UserService,
    private readonly reviewService: ReviewService) {
    this.reviewForm = this.fb.group({
      description: ['', [Validators.required, Validators.maxLength(500)]],
      rating: ['', [Validators.required, Validators.min(1), Validators.max(10), Validators.pattern('^[1-9]$|^10$')]]
    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.reviewForm.reset();
  }

  onSubmit(): void {
    if (this.reviewForm.valid) {
      const formValue = this.reviewForm.value;

      formValue.buyerId = this.userService.getUserId();
      formValue.sellerId = this.route.snapshot.paramMap.get('id');

      this.reviewService.createReview(formValue).subscribe(() => {
        this.router.navigate(['review-users/get-all', this.reviewForm.value.sellerId]);
      });
    }
  }
}