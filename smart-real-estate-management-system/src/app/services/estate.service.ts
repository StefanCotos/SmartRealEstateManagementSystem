import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { forkJoin, Observable } from 'rxjs';
import { Estate } from '../models/estate.model';

@Injectable({
  providedIn: 'root',
})
export class EstateService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/estates';
  private readonly stripeURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/stripe';
  private readonly predictionURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/estate-price-prediction/predict';
  // private readonly apiURL = 'http://localhost:5045/api/estates';
  // private readonly stripeURL = 'http://localhost:5045/api/stripe';
  // private readonly predictionURL = 'http://localhost:5045/api/estate-price-prediction/predict';

  constructor(private readonly http: HttpClient) { }

  getPaginatedEstates(
    filters: {
      name: string,
      street: string,
      city: string,
      state: string,
      price: number,
      bedrooms: number,
      bathrooms: number,
      landSize: number,
      houseSize: number,
    },
    pagination: {
      page: number,
      pageSize: number
    }
  ): Observable<any> {
    let params = new HttpParams()
      .set('name', filters.name)
      .set('street', filters.street)
      .set('city', filters.city)
      .set('state', filters.state)
      .set('price', filters.price.toString())
      .set('bedrooms', filters.bedrooms.toString())
      .set('bathrooms', filters.bathrooms.toString())
      .set('landSize', filters.landSize.toString())
      .set('houseSize', filters.houseSize.toString())
      .set('page', pagination.page.toString())
      .set('pageSize', pagination.pageSize.toString());

    return this.http.get<any>(`${this.apiURL}/filter/paginated`, { params });
  }

  public getPaginatedFilterEstates(
    pageNumber: number,
    pageSize: number
  ): Observable<Estate[]> {
    const paginatedUrl = `${this.apiURL}/filter/paginated?page=${pageNumber}&pageSize=${pageSize}`;
    return this.http.get<Estate[]>(paginatedUrl);
  }

  public createEstate(estate: Estate): Observable<Estate> {
    const token = localStorage.getItem('token');

    const headers = token ? { Authorization: `Bearer ${token}` } : undefined;

    return this.http.post<Estate>(this.apiURL, estate, { headers });
  }

  public createProductStripe(
    name: string,
    description: string,
    unitAmount: number
  ): Observable<any> {
    return this.http.post(`${this.stripeURL}/createProduct`, {
      name,
      description,
      unitAmount,
    });
  }

  public updateEstate(estate: Estate): Observable<Estate> {
    const token = localStorage.getItem('token');

    const headers = token ? { Authorization: `Bearer ${token}` } : undefined;

    return this.http.put<Estate>(`${this.apiURL}/${estate.id}`, estate, {
      headers,
    });
  }

  public getAllEstates(): Observable<Estate[]> {
    return this.http.get<Estate[]>(this.apiURL);
  }

  public getEstateById(id: string): Observable<Estate> {
    return this.http.get<Estate>(`${this.apiURL}/${id}`);
  }

  public deleteEstate(id: string): Observable<any> {
    const token = localStorage.getItem('token');

    const headers = token ? { Authorization: `Bearer ${token}` } : undefined;

    return this.http.delete(`${this.apiURL}/${id}`, { headers });
  }

  public estimateEstatePrice(estate: Estate): Observable<any> {
    return this.http.post<any>(
      `${this.predictionURL}`,
      estate
    );
  }

  public buyProduct(priceId: string): Observable<any> {
    const body = { defaultPriceId: priceId };
    return this.http.post(`${this.stripeURL}/pay`, body, {
      responseType: 'text',
    });
  }

  public updateEstateProduct(estateDetails: any): Observable<any> {
    return this.http.put(`${this.stripeURL}/updateProduct`, estateDetails);
  }

  getEstatesByIds(ids: string[]): Observable<Estate[]> {
    const estateObservables = ids.map((id) => this.getEstateById(id));
    return forkJoin(estateObservables);
  }

  getEstatesByUserId(userId: string): Observable<any> {
    return this.http.get<any>(`${this.apiURL}/users/${userId}`);
  }

  getEstatesByBuyerId(userId: string): Observable<any> {
    return this.http.get<any>(`${this.apiURL}/buyers/${userId}`);
  }

}
