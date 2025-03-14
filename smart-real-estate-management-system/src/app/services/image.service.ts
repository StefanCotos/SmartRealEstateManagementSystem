import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Image } from '../models/image.model';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/images';
  // private readonly apiURL = 'http://localhost:5045/api/images';

  constructor(private readonly http: HttpClient) { }

  saveImage(image: Image): Observable<Image> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<Image>(`${this.apiURL}`, image, { headers });
  }

  getImagesByEstateId(estateId: string): Observable<Image[]> {
    return this.http.get<Image[]>(`${this.apiURL}/estates/${estateId}`);
  }
}
