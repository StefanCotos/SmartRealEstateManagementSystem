import { TestBed } from '@angular/core/testing';

import { TokenExpirationInterceptor } from '../../services/interceptors/token-expiration.service';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpTestingController } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserService } from '../user.service';

describe('TokenExpirationInterceptor', () => {
  let service: TokenExpirationInterceptor;
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getTokenExpirationDate', 'logout']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [],
      providers: [
        TokenExpirationInterceptor,
        { provide: HTTP_INTERCEPTORS, useClass: TokenExpirationInterceptor, multi: true },
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting()
      ],
    });

    service = TestBed.inject(TokenExpirationInterceptor);
    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should not logout if token is not expired', () => {
    const futureDate = new Date(Date.now() + 10000);
    userService.getTokenExpirationDate.and.returnValue(futureDate);
    localStorage.setItem('token', 'fake-token');

    httpClient.get('/test').subscribe(response => {
      expect(response).toBeTruthy();
      expect(userService.logout).not.toHaveBeenCalled();
      expect(router.navigate).not.toHaveBeenCalled();
    });

    const req = httpMock.expectOne('/test');
    req.flush({ data: 'test' });
  });

  it('should logout if token is expired', () => {
    const pastDate = new Date(Date.now() - 10000);
    userService.getTokenExpirationDate.and.returnValue(pastDate);
    localStorage.setItem('token', 'fake-token');

    httpClient.get('/test').subscribe({
      error: (error) => {
        expect(error).toBeTruthy();
      },
    });

    // Since the interceptor blocks the request, there's no actual request to match
    expect(userService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should pass the request unchanged if no token is present', () => {
    localStorage.removeItem('token');

    httpClient.get('/test').subscribe(response => {
      expect(response).toBeTruthy();
    });

    const req = httpMock.expectOne('/test');
    expect(req.request.method).toBe('GET');
    req.flush({ data: 'success' });
  });

  it('should logout and redirect if a 401 error occurs for a non-expired token', () => {
    const futureDate = new Date(Date.now() + 10000);
    userService.getTokenExpirationDate.and.returnValue(futureDate);
    localStorage.setItem('token', 'valid-token');

    httpClient.get('/test').subscribe({
      error: (err) => {
        expect(err.status).toBe(401);
      },
    });

    const req = httpMock.expectOne('/test');
    req.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(userService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should not logout or redirect for non-401 errors', () => {
    const futureDate = new Date(Date.now() + 10000);
    userService.getTokenExpirationDate.and.returnValue(futureDate);
    localStorage.setItem('token', 'valid-token');

    httpClient.get('/test').subscribe({
      error: (err) => {
        expect(err.status).toBe(500);
      },
    });

    const req = httpMock.expectOne('/test');
    req.flush(null, { status: 500, statusText: 'Internal Server Error' });

    expect(userService.logout).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });

});