import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError } from 'rxjs';
import { User } from '../models/user.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly apiURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/auth';
  private readonly usersURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/users';
  private readonly resetPasswordURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/reset-password';
  private readonly actionsOnUserURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/actions-on-user';
  private readonly contactURL = 'https://smart-real-estate-management-227505df1e15.herokuapp.com/api/contact';
  // private readonly apiURL = 'http://localhost:5045/api/auth';
  // private readonly usersURL = 'http://localhost:5045/api/users';
  // private readonly resetPasswordURL = 'http://localhost:5045/api/reset-password';
  // private readonly actionsOnUserURL = 'http://localhost:5045/api/actions-on-user';
  // private readonly contactURL = 'http://localhost:5045/api/contact';


  isUserLoggedIn: boolean = false;
  userName: string | null = null;
  userId: string = '';

  constructor(private readonly http: HttpClient) {
    this.initializeUserFromToken();
  }

  private initializeUserFromToken(): void {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken = jwtDecode<any>(token);
      this.userName = decodedToken['unique_name'] || null;
      this.userId = decodedToken['nameid'] || '';
    }
  }

  public register(user: User): Observable<User> {
    return this.http.post<User>(`${this.apiURL}/register`, user).pipe(
      tap(() => {
      }),
      catchError((error) => {
        throw error;
      })
    );
  }

  public login(user: User): Observable<any> {
    return this.http.post<any>(`${this.apiURL}/login`, user).pipe(
      tap((response: any) => {
        const token = response.token;
        localStorage.setItem('token', token);

        const decodedToken = jwtDecode<any>(token);
        this.userName = decodedToken['unique_name'] || null;
        this.userId = decodedToken['nameid'] || '';
        localStorage.setItem('isAdmin', decodedToken['role']);
      }),
      catchError((error) => {
        throw error;
      })
    );
  }

  public sendContactForm(contactData: { name: string; email: string; subject: string; message: string }): Observable<any> {
    return this.http.post<any>(`${this.contactURL}`, contactData);
  }

  public getTokenExpirationDate(token: string): Date | null {
    const decodedToken = jwtDecode<any>(token);
    if (!decodedToken?.exp) {
      return null;
    }

    const date = new Date(0);
    date.setUTCSeconds(decodedToken.exp);
    return date;
  }

  public isAdmin(): boolean {
    const role = localStorage.getItem('isAdmin');
    return role === 'Admin';
  }

  public requestSendLink(email: string): Observable<any> {
    return this.http.post<any>(`${this.resetPasswordURL}/forgot-password`, { email });
  }

  public requestPasswordReset(token: string, password: string): Observable<any> {
    const body = { token: token, newPassword: password };
    return this.http.post<any>(`${this.resetPasswordURL}/reset-password`, body); // Endpoint-ul backend-ului
  }

  public confirmEmailValid(token: string): Observable<any> {
    return this.http.post<any>(`${this.apiURL}/email-confirmation-valid`, { token });
  }

  public logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('isAdmin');
    this.userName = null;
    this.userId = '';
    this.isUserLoggedIn = false;
  }

  public isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    if (!token) {
      return false;
    }

    const expirationDate = this.getTokenExpirationDate(token);
    return expirationDate ? expirationDate > new Date() : false;
  }

  public getUserName(): string | null {
    return this.userName;
  }

  public getUserId(): string {
    return this.userId;
  }

  public getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.usersURL}/${id}`);
  }

  public deleteUser(id: string): Observable<any> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.delete(`${this.actionsOnUserURL}/${id}`, { headers });
  }

  public removeUser(id: string): Observable<any> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    localStorage.removeItem('token');
    this.userName = null;
    this.userId = '';
    this.isUserLoggedIn = false;
    return this.http.delete(`${this.actionsOnUserURL}/${id}`, { headers });
  }

  public editUser(user: User): Observable<User> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.put<User>(`${this.actionsOnUserURL}/${user.id}`, user, { headers }).pipe(
      tap((response: any) => {
        const newToken = response.token;
        localStorage.setItem('token', newToken);
        this.initializeUserFromToken();
      }),
      catchError((error) => {
        throw error;
      })
    );
  }

  public checkPassword(user: User): Observable<boolean> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<boolean>(`${this.actionsOnUserURL}/check-password`, user, { headers }).pipe(
      tap((response: any) => {
        return response;
      }),
      catchError((error) => {
        throw error;
      })
    );
  }

  public changePassword(user: User): Observable<boolean> {
    const token = localStorage.getItem('token');

    const headers = token
      ? { Authorization: `Bearer ${token}` }
      : undefined;

    return this.http.post<boolean>(`${this.actionsOnUserURL}/change-password`, user, { headers }).pipe(
      tap((response: any) => {
        return response;
      }),
      catchError((error) => {
        throw error;
      })
    );
  }

  public getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.usersURL).pipe(
      tap(users => {
      }),
      catchError((error) => {
        console.error('Failed to fetch users', error);
        throw error;
      })
    );
  }


}
