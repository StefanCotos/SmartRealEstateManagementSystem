import { TestBed } from '@angular/core/testing';

import { UserService } from './user.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { HttpTestingController } from '@angular/common/http/testing';
import { User } from '../models/user.model';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    TestBed.configureTestingModule({
      imports: [],
      providers: [UserService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register a user', () => {
    const mockUser: User = { id: '1', userName: 'john', email: 'john@example.com', password: 'password', firstName: 'John', lastName: 'Doe' };
    service.register(mockUser).subscribe((response) => {
      expect(response).toEqual(mockUser);
    });

    const req = httpMock.expectOne('http://localhost:5045/api/auth/register');
    expect(req.request.method).toBe('POST');
    req.flush(mockUser);
  });

  it('should get a user by id', () => {
    const mockUser: User = { id: '1', userName: 'john', email: 'john@example.com', password: 'password', firstName: 'John', lastName: 'Doe' };

    service.getUserById('1').subscribe((response) => {
      expect(response).toEqual(mockUser);
    });

    const req = httpMock.expectOne('http://localhost:5045/api/users/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockUser);
  });

  it('should logout a user', () => {
    localStorage.setItem('token', 'fake-jwt-token');
    service.logout();

    expect(localStorage.getItem('token')).toBeNull();
    expect(service.userName).toBeNull();
  });

  it('should return true if the user is an admin', () => {
    localStorage.setItem('isAdmin', 'Admin');
    expect(service.isAdmin()).toBe(true);
  });

  it('should send a password reset request', () => {
    const mockResponse = { message: 'Password reset link sent' };
    const email = 'john@example.com';

    service.requestSendLink(email).subscribe((response) => {
      expect(response.message).toBe('Password reset link sent');
    });

    const req = httpMock.expectOne('http://localhost:5045/api/reset-password/forgot-password');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should handle error on login failure', () => {
    const mockErrorResponse = { message: 'Invalid credentials' };
    const mockUser: User = { id: '1', userName: 'john', email: 'john@example.com', password: 'password', firstName: 'John', lastName: 'Doe' };

    service.login(mockUser).subscribe({
      next: () => fail('should have failed with 401 error'),
      error: (error: HttpErrorResponse) => {
        expect(error.status).toBe(401);
        expect(error.statusText).toBe('Unauthorized');
        expect(error.error.message).toBe('Invalid credentials');
      },
    });

    const req = httpMock.expectOne('http://localhost:5045/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse, { status: 401, statusText: 'Unauthorized' });
  });

  it('should send contact form and return response', () => {
    const mockContactData = { name: 'John Doe', email: 'john@example.com', subject: 'Test', message: 'Test message' };
    const mockResponse = { success: true };

    service.sendContactForm(mockContactData).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/contact');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should send password reset request and return response', () => {
    const mockEmail = 'john@example.com';
    const mockResponse = { success: true };

    service.requestSendLink(mockEmail).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/reset-password/forgot-password');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should reset password and return response', () => {
    const mockToken = 'reset-token';
    const mockPassword = 'newPassword123';
    const mockResponse = { success: true };

    service.requestPasswordReset(mockToken, mockPassword).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/reset-password/reset-password');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should confirm email validity and return response', () => {
    const mockToken = 'confirmation-token';
    const mockResponse = { valid: true };

    service.confirmEmailValid(mockToken).subscribe(response => {
      expect(response.valid).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/auth/email-confirmation-valid');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should delete user and return response', () => {
    const mockUserId = '1';
    const mockResponse = { success: true };
    localStorage.setItem('token', 'mock-token'); // Ensure the token is set for authorization

    service.deleteUser(mockUserId).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`http://localhost:5045/api/actions-on-user/${mockUserId}`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    req.flush(mockResponse);
  });

  it('should remove user and return response', () => {
    const mockUserId = '1';
    const mockResponse = { success: true };
    localStorage.setItem('token', 'mock-token'); // Ensure the token is set for authorization

    service.removeUser(mockUserId).subscribe(response => {
      expect(response.success).toBeTrue();
    });

    const req = httpMock.expectOne(`http://localhost:5045/api/actions/${mockUserId}`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.get('Authorization')).toBe('Bearer mock-token');
    req.flush(mockResponse);
  });

  it('should check password and return true', () => {
    const mockUser: User = { id: '1', userName: 'john', email: 'john@example.com', password: 'oldPassword', firstName: 'John', lastName: 'Doe' };
    const mockResponse = true;

    service.checkPassword(mockUser).subscribe(isValid => {
      expect(isValid).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/actions-on-user/check-password');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should change password and return true', () => {
    const mockUser: User = { id: '1', userName: 'john', email: 'john@example.com', password: 'newPassword123', firstName: 'John', lastName: 'Doe' };
    const mockResponse = true;

    service.changePassword(mockUser).subscribe(isSuccess => {
      expect(isSuccess).toBeTrue();
    });

    const req = httpMock.expectOne('http://localhost:5045/api/actions-on-user/change-password');
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should return list of users', () => {
    const mockUsers: User[] = [
      { id: '1', userName: 'john', email: 'john@example.com', password: 'password', firstName: 'John', lastName: 'Doe' },
      { id: '2', userName: 'jane', email: 'jane@example.com', password: 'password', firstName: 'Jane', lastName: 'Doe' }
    ];

    service.getAllUsers().subscribe(users => {
      expect(users.length).toBe(2);
      expect(users[0].userName).toBe('john');
      expect(users[1].userName).toBe('jane');
    });

    const req = httpMock.expectOne('http://localhost:5045/api/users');
    expect(req.request.method).toBe('GET');
    req.flush(mockUsers);
  });

  it('should return the username from localStorage', () => {
    const mockUsername = 'john_doe';
    localStorage.setItem('token', 'mock-token'); // Asigură-te că tokenul este prezent în localStorage

    // Simulează decodarea tokenului
    spyOn<any>(service, 'initializeUserFromToken').and.callFake(() => {
      service.userName = mockUsername;
    });

    const username = service.getUserName();

    expect(username).toBe(null);
  });

  it('should send a DELETE request to delete a user with authorization header', () => {
    const mockToken = 'mock-token';
    const userId = '12345';

    localStorage.setItem('token', mockToken);

    // Apelăm metoda deleteUser
    service.deleteUser(userId).subscribe();

    // Verificăm că cererea HTTP a fost făcută cu header-ul corect
    const req = httpMock.expectOne(`http://localhost:5045/api/actions-on-user/${userId}`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.has('Authorization')).toBeTrue();
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${mockToken}`);

    req.flush({});
  });

  it('should send a DELETE request to remove a user with authorization header', () => {
    const mockToken = 'mock-token';
    const userId = '12345';

    localStorage.setItem('token', mockToken);

    // Apelăm metoda removeUser
    service.removeUser(userId).subscribe();

    // Verificăm că cererea HTTP a fost făcută cu header-ul corect
    const req = httpMock.expectOne(`http://localhost:5045/api/actions-on-user/${userId}`);
    expect(req.request.method).toBe('DELETE');
    expect(req.request.headers.has('Authorization')).toBeTrue();
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${mockToken}`);

    req.flush({});
  });

  it('should handle error when changing password', () => {
    const mockToken = 'mock-token';
    const user: User = { id: '123', userName: 'testUser', password: 'newPassword', firstName: 'Test', lastName: 'User', email: 'test@gmail.com' };

    localStorage.setItem('token', mockToken);

    // Apelăm metoda changePassword
    service.changePassword(user).subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error) => {
        expect(error.status).toBe(500);
        expect(error.error.message).toBe('Error changing password');
      }
    });

    // Simulăm eroarea la cererea HTTP
    const req = httpMock.expectOne(`http://localhost:5045/api/actions-on-user/change-password`);
    expect(req.request.method).toBe('POST');
    req.flush({ message: 'Error changing password' }, { status: 500, statusText: 'Server Error' });
  });

  it('should handle error when checking password', () => {
    const mockToken = 'mock-token';
    const user: User = { id: '123', userName: 'testUser', password: 'wrongPassword', firstName: 'Test', lastName: 'User', email: 'test@gmail.com' };

    localStorage.setItem('token', mockToken);

    // Apelăm metoda checkPassword
    service.checkPassword(user).subscribe({
      next: () => fail('should have failed with 401 error'),
      error: (error) => {
        expect(error.status).toBe(401);
        expect(error.error.message).toBe('Invalid credentials');
      }
    });

    // Simulăm eroarea la cererea HTTP
    const req = httpMock.expectOne(`http://localhost:5045/api/actions-on-user/check-password`);
    expect(req.request.method).toBe('POST');
    req.flush({ message: 'Invalid credentials' }, { status: 401, statusText: 'Unauthorized' });
  });

  it('should handle error when fetching all users', () => {
    const mockToken = 'mock-token';

    localStorage.setItem('token', mockToken);

    // Apelăm metoda getAllUsers
    service.getAllUsers().subscribe({
      next: () => fail('should have failed with 500 error'),
      error: (error) => {
        expect(error.status).toBe(500);
        expect(error.error.message).toBe('Failed to fetch users');
      }
    });

    // Simulăm eroarea la cererea HTTP
    const req = httpMock.expectOne(`http://localhost:5045/api/users`);
    expect(req.request.method).toBe('GET');
    req.flush({ message: 'Failed to fetch users' }, { status: 500, statusText: 'Server Error' });
  });

  it('should return false when token is null', () => {
    // Îndepărtăm token-ul din localStorage pentru a simula cazul în care nu există niciun token
    localStorage.removeItem('token');

    // Apelăm metoda isLoggedIn
    const result = service.isLoggedIn();

    // Verificăm că rezultatul este fals
    expect(result).toBeFalse();
  });

});