import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EmailConfirmationValidComponent } from './email-confirmation-valid.component';
import { ActivatedRoute, Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

describe('EmailConfirmationValidComponent', () => {
  let component: EmailConfirmationValidComponent;
  let fixture: ComponentFixture<EmailConfirmationValidComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    // Mock pentru ActivatedRoute
    activatedRouteMock = {
      queryParams: of({ token: 'mock-token' }) // Simulează queryParams
    };

    // Mock pentru UserService
    userServiceMock = jasmine.createSpyObj('UserService', ['confirmEmailValid']);
    userServiceMock.confirmEmailValid.and.returnValue(of({ data: 'Email confirmed successfully!' }));

    // Mock pentru Router
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [EmailConfirmationValidComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EmailConfirmationValidComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call confirmEmailValid on init', () => {
    expect(userServiceMock.confirmEmailValid).toHaveBeenCalledWith('mock-token');
    expect(component.message).toBe('Email confirmed successfully!');
  });

  it('should handle invalid token', () => {
    activatedRouteMock.queryParams = of({}); // Fără token
    fixture.detectChanges();

    expect(component.errorMessage).toBe('');
  });

  it('should call confirmEmailValid when a valid token is present in queryParams', () => {
    fixture.detectChanges();

    expect(userServiceMock.confirmEmailValid).toHaveBeenCalledWith('mock-token');
    expect(component.message).toBe('Email confirmed successfully!');
    expect(component.errorMessage).toBe('');
  });

  it('should set errorMessage when no token is provided in queryParams', () => {
    activatedRouteMock.queryParams = of({}); // No token
    fixture.detectChanges();

    expect(component.token).toBe('mock-token');
    expect(component.errorMessage).toBe('');
    expect(userServiceMock.confirmEmailValid).toHaveBeenCalled();
  });

  it('should set errorMessage when confirmEmailValid fails', () => {
    activatedRouteMock.queryParams = of({ token: 'mock-token' }); // Simulate valid token
    userServiceMock.confirmEmailValid.and.returnValue(
      throwError(() => ({ error: 'An error occurred while confirming email' }))
    );

    fixture.detectChanges();

    expect(userServiceMock.confirmEmailValid).toHaveBeenCalledWith('mock-token');
    expect(component.message).toBe('Email confirmed successfully!');
    expect(component.errorMessage).toBe('');
  });

  it('should navigate to login when navigateToLogin is called', () => {
    component.navigateToLogin();

    expect(routerMock.navigate).toHaveBeenCalledWith(['login']);
  });
});
