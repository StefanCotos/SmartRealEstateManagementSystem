import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ForgotPasswordComponent } from './forgot-password.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { UserService } from '../../services/user.service';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';

describe('ForgotPasswordComponent', () => {
  let component: ForgotPasswordComponent;
  let fixture: ComponentFixture<ForgotPasswordComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['requestSendLink', 'isLoggedIn', 'isAdmin']);

    await TestBed.configureTestingModule({
      imports: [ForgotPasswordComponent, ReactiveFormsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: UserService, useValue: userServiceSpy }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ForgotPasswordComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a form with email control', () => {
    expect(component.forgotPasswordForm.contains('email')).toBeTruthy();
  });

  it('should make the email control required', () => {
    const control = component.forgotPasswordForm.get('email');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should display a success message when the form is submitted successfully', () => {
    const mockResponse = { data: 'Success message' };
    userService.requestSendLink.and.returnValue(of(mockResponse));

    component.forgotPasswordForm.setValue({ email: 'test@example.com' });
    component.onSubmit();

    expect(component.message).toBe(mockResponse.data);
    expect(component.errorMessage).toBe('');
  });

  it('should display an error message when the form submission fails', () => {
    const mockError = { error: 'Error message' };
    userService.requestSendLink.and.returnValue(throwError(() => mockError));

    component.forgotPasswordForm.setValue({ email: 'test@example.com' });
    component.onSubmit();

    expect(component.errorMessage).toBe(mockError.error);
    expect(component.message).toBe('');
  });

  it('should not submit the form if email is invalid', () => {
    component.forgotPasswordForm.setValue({ email: 'invalid-email' });
    component.onSubmit();

    expect(userService.requestSendLink).not.toHaveBeenCalled();
  });

  it('should disable submit button if form is invalid', () => {
    component.forgotPasswordForm.setValue({ email: '' });
    fixture.detectChanges();

    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBeTruthy();
  });
});