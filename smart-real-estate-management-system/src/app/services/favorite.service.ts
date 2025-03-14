import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Favorite } from '../models/favorite.model';

@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/favorites';
  // private readonly apiURL = 'http://localhost:5045/api/favorites';

  constructor(private readonly http: HttpClient) { }

  saveFavorite(favorite: Favorite): Observable<Favorite> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<Favorite>(`${this.apiURL}`, favorite, { headers });
  }

  deleteFavorite(userId: string, estateId: string): Observable<Favorite> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.delete<Favorite>(`${this.apiURL}?userId=${userId}&estateId=${estateId}`, { headers });
  }

  getFavoritesByUserId(userId: string): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(`${this.apiURL}/users/${userId}`);
  }
}
