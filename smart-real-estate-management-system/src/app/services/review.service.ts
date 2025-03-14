import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review } from '../models/review.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/review-users';
  // private readonly apiURL = 'http://localhost:5045/api/review-users';

  constructor(private readonly http: HttpClient) { }

  getSellerReviews(id: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.apiURL}/sellers/${id}`);
  }

  createReview(review: Review): Observable<Review> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<Review>(`${this.apiURL}`, review, { headers });
  }
}
