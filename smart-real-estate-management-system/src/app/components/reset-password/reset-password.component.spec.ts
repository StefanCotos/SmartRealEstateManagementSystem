import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResetPasswordComponent } from './reset-password.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

describe('ResetPasswordComponent', () => {
  let component: ResetPasswordComponent;
  let fixture: ComponentFixture<ResetPasswordComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        get: jasmine.createSpy('get').and.returnValue('1'),
        params: { token: 'test-token' }
      },
      queryParams: of({ token: 'test-token' })
    };

    userServiceMock = {
      requestPasswordReset: jasmine.createSpy('requestPasswordReset').and.returnValue(of({ data: 'Password reset successful' }))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [ResetPasswordComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        FormBuilder,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ResetPasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with token from query params', () => {
    expect(component.token).toBe('test-token');
  });

  it('should show error message if token is missing', () => {
    activatedRouteMock.queryParams = of({});
    component.ngOnInit();
    expect(component.errorMessage).toBe('Invalid or missing token');
  });

  it('should call userService.requestPasswordReset on valid form submission', () => {
    component.resetPasswordForm.setValue({ password: 'password123', confirmPassword: 'password123' });
    component.onSubmit();
    expect(userServiceMock.requestPasswordReset).toHaveBeenCalledWith('test-token', 'password123');
  });

  it('should navigate to login on successful password reset', () => {
    component.resetPasswordForm.setValue({ password: 'password123', confirmPassword: 'password123' });
    component.onSubmit();
    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should show error message on failed password reset', () => {
    userServiceMock.requestPasswordReset.and.returnValue(throwError({ error: 'Password reset failed' }));
    component.resetPasswordForm.setValue({ password: 'password123', confirmPassword: 'password123' });
    component.onSubmit();
    expect(component.errorMessage).toBe('Password reset failed');
  });
});
