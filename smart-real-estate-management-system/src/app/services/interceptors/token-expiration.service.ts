import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { UserService } from '../user.service';

@Injectable()
export class TokenExpirationInterceptor implements HttpInterceptor {

  constructor(private readonly userService: UserService, private readonly router: Router) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = localStorage.getItem('token');

    if (token) {
      const expirationDate = this.userService.getTokenExpirationDate(token);
      const isExpired = expirationDate ? expirationDate < new Date() : true;

      if (isExpired) {
        this.userService.logout();
        this.router.navigate(['/login']);
        return throwError(() => new Error('Token has expired'));
      }
    }

    return next.handle(req).pipe(
      catchError(err => {
        if (err.status === 401) {
          this.userService.logout();
          this.router.navigate(['/login']);
        }
        return throwError(() => err);
      })
    );
  }
}