import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { ReviewService } from '../../services/review.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { Review } from '../../models/review.model';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-user-see-reviews',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './user-see-reviews.component.html',
  styleUrl: './user-see-reviews.component.css'
})
export class UserSeeReviewsComponent implements OnInit {
  reviews: Review[] = [];
  username: string | null = '';
  sellerId: string | null = '';
  buyersUsername: { [key: string]: string } = {};

  constructor(private readonly fb: FormBuilder,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly userService: UserService,
    private readonly reviewService: ReviewService) {

  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.sellerId = this.route.snapshot.paramMap.get('id');
    if (this.sellerId) {
      this.reviewService.getSellerReviews(this.sellerId).subscribe((reviews: Review[]) => {
        this.reviews = reviews;
        this.reviews.forEach(review => {
          this.userService.getUserById(review.buyerId).subscribe(data => {
            this.buyersUsername[review.buyerId] = data.userName;
          });
        });
      });
      this.userService.getUserById(this.sellerId).subscribe(data => {
        this.username = data.userName;
      });
    }
  }

  navigateToAddReview(): void {
    this.router.navigate(['review-users/create', this.sellerId]);
  }

  ifCanAddReview(): boolean {
    return this.userService.getUserId() !== this.sellerId;
  }

}
