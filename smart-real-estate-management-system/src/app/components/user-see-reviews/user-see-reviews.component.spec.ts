import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { UserSeeReviewsComponent } from './user-see-reviews.component';
import { UserService } from '../../services/user.service';
import { ReviewService } from '../../services/review.service'
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('UserSeeReviewsComponent', () => {
  let component: UserSeeReviewsComponent;
  let fixture: ComponentFixture<UserSeeReviewsComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let reviewServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    userServiceMock = {
      getUserId: jasmine.createSpy('getUserId').and.returnValue('user123'),
      getUserById: jasmine.createSpy('getUserById').and.returnValue(of({ userName: 'sellerUser' })),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false),
      getUserName: jasmine.createSpy('getUserName').and.returnValue(of('sellerUser'))
    };

    reviewServiceMock = {
      getSellerReviews: jasmine.createSpy('getSellerReviews').and.returnValue(of([
        { buyerId: 'user123', reviewText: 'Great product!' },
        { buyerId: 'user456', reviewText: 'Nice experience.' }
      ])),
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };


    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [UserSeeReviewsComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ReviewService, useValue: reviewServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserSeeReviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch reviews and seller information on ngOnInit', () => {
    // Check that sellerId is set correctly
    expect(component.sellerId).toBe('1');

    // Verify that reviewService.getSellerReviews is called with sellerId
    expect(reviewServiceMock.getSellerReviews).toHaveBeenCalledWith('1');

    // Verify that userService.getUserById is called for each buyerId in the reviews
    expect(userServiceMock.getUserById).toHaveBeenCalledWith('user123');
    expect(userServiceMock.getUserById).toHaveBeenCalledWith('user456');

    // Verify that the reviews are correctly assigned
    expect(component.reviews.length).toBe(2);
    expect(component.reviews[0].buyerId).toBe('user123');
    expect(component.reviews[1].buyerId).toBe('user456');

    // Verify that the buyers' usernames are set correctly
    expect(component.buyersUsername['user123']).toBe('sellerUser');
    expect(component.buyersUsername['user456']).toBe('sellerUser');
    
    // Verify that the seller's username is correctly assigned
    expect(component.username).toBe('sellerUser');
  });

  it('should navigate to "review-users/create" when navigateToAddReview is called', () => {
    component.navigateToAddReview();
    expect(routerMock.navigate).toHaveBeenCalledWith(['review-users/create', '1']);
  });

  it('should return true if current user can add review', () => {
    expect(component.ifCanAddReview()).toBe(true); // Assuming 'user123' is not the sellerId
  });

  it('should return false if current user is the seller and cannot add review', () => {
    userServiceMock.getUserId.and.returnValue('1'); // Current user is the seller
    expect(component.ifCanAddReview()).toBe(false);
  });
});
