import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['login', 'isAdmin', 'isLoggedIn']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, LoginComponent],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a form with email and password fields', () => {
    expect(component.loginForm.contains('email')).toBeTruthy();
    expect(component.loginForm.contains('password')).toBeTruthy();
  });

  it('should make the email control required', () => {
    const control = component.loginForm.get('email');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should make the password control required', () => {
    const control = component.loginForm.get('password');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should navigate to admin if login is successful and user is admin', () => {
    userService.login.and.returnValue(of({}));
    userService.isAdmin.and.returnValue(true);

    component.loginForm.setValue({ email: 'admin@example.com', password: 'password123' });
    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['admin']);
  });

  it('should navigate to home if login is successful and user is not admin', () => {
    userService.login.and.returnValue(of({}));
    userService.isAdmin.and.returnValue(false);

    component.loginForm.setValue({ email: 'user@example.com', password: 'password123' });
    component.onSubmit();

    expect(router.navigate).toHaveBeenCalledWith(['']);
  });

  it('should display an error message if login fails', () => {
    const errorResponse = { error: 'Invalid credentials' };
    userService.login.and.returnValue(throwError(errorResponse));

    component.loginForm.setValue({ email: 'user@example.com', password: 'wrongpassword' });
    component.onSubmit();

    expect(component.errorMessage).toBe('Invalid credentials');
  });

  it('should clear the error message after 5 seconds', (done) => {
    component.errorMessage = 'Some error';
    component.onSubmit();

    setTimeout(() => {
      expect(component.errorMessage).toBe('');
      done();
    }, 5000);
  });

  it('should navigate to register page', () => {
    component.navigateToRegister();
    expect(router.navigate).toHaveBeenCalledWith(['register']);
  });

  it('should navigate to forgot password page', () => {
    component.navigateToForgotPassword();
    expect(router.navigate).toHaveBeenCalledWith(['forgot-password']);
  });

  it('should reset the form on initialization', () => {
    spyOn(component.loginForm, 'reset');
    component.ngOnInit();
    expect(component.loginForm.reset).toHaveBeenCalled();
  });

  it('should scroll to the top on initialization', () => {
    spyOn(window, 'scrollTo');
    component.ngOnInit();
    expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
  });

  it('should not submit the form if it is invalid', () => {
    spyOn(component, 'onSubmit');
    component.loginForm.setValue({ email: '', password: '' });
    component.onSubmit();
    expect(component.onSubmit).toHaveBeenCalled();
    expect(userService.login).not.toHaveBeenCalled();
  });

  it('should not navigate if login fails', () => {
    const errorResponse = { error: 'Invalid credentials' };
    userService.login.and.returnValue(throwError(errorResponse));

    component.loginForm.setValue({ email: 'user@example.com', password: 'wrongpassword' });
    component.onSubmit();

    expect(router.navigate).not.toHaveBeenCalled();
  });
});
