import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserReviewComponent } from './user-review.component';
import { ActivatedRoute } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { UserService } from '../../services/user.service';
import { ReviewService } from '../../services/review.service';

describe('UserReviewComponent', () => {
  let component: UserReviewComponent;
  let fixture: ComponentFixture<UserReviewComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let reviewServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    userServiceMock = {
      getUserId: jasmine.createSpy('getUserId').and.returnValue('123'),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('John Doe'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
    };

    reviewServiceMock = {
      createReview: jasmine.createSpy('createReview').and.returnValue(of({}))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, UserReviewComponent],
      providers: [
        FormBuilder,
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ReviewService, useValue: reviewServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserReviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form on init', () => {
    component.ngOnInit();
    expect(component.reviewForm).toBeDefined();
    expect(component.reviewForm.controls['description'].value).toBe(null);
    expect(component.reviewForm.controls['rating'].value).toBe(null);
  });

  it('should not submit the form if it is invalid', () => {
    component.reviewForm.controls['description'].setValue('');
    component.reviewForm.controls['rating'].setValue('');
    component.onSubmit();
    expect(reviewServiceMock.createReview).not.toHaveBeenCalled();
  });

  it('should submit the form if it is valid', () => {
    component.reviewForm.controls['description'].setValue('Great service!');
    component.reviewForm.controls['rating'].setValue('10');
    component.onSubmit();
    expect(reviewServiceMock.createReview).toHaveBeenCalledWith({
      description: 'Great service!',
      rating: '10',
      buyerId: '123',
      sellerId: '1'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['review-users/get-all', '1']);
  });
});
