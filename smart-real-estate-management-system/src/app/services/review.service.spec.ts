import { TestBed } from '@angular/core/testing';

import { ReviewService } from './review.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpTestingController } from '@angular/common/http/testing';
import { Review } from '../models/review.model';

describe('ReviewService', () => {
  let service: ReviewService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [ReviewService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ReviewService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch seller reviews', () => {
    const dummyReviews: Review[] = [
      { id: '1', buyerId: '2', sellerId: '3', description: 'description', rating: 5 },
      { id: '2', buyerId: '2', sellerId: '3', description: 'description', rating: 3 }
    ];

    service.getSellerReviews('123').subscribe(reviews => {
      expect(reviews.length).toBe(2);
      expect(reviews).toEqual(dummyReviews);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/sellers/123`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyReviews);
  });

  it('should create a new review', () => {
    const newReview: Review = { id: '3', buyerId: '2', sellerId: '3', description: 'description', rating: 5 };

    service.createReview(newReview).subscribe(review => {
      expect(review).toEqual(newReview);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    req.flush(newReview);
  });

  it('should not create a new review if user is not logged in', () => {
    const newReview: Review = { id: '3', buyerId: '2', sellerId: '3', description: 'description', rating: 5 };

    localStorage.removeItem('token');

    service.createReview(newReview).subscribe(review => {
      expect(review).toEqual(newReview);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush(newReview);
  });

  it('should create a new review with authorization', () => {
    const newReview: Review = { id: '3', buyerId: '2', sellerId: '3', description: 'description', rating: 5 };
    const token = 'dummy-token';
    localStorage.setItem('token', token);

    service.createReview(newReview).subscribe(review => {
      expect(review).toEqual(newReview);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(newReview);
  });
});