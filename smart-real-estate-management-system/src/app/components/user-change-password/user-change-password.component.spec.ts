import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserChangePasswordComponent } from './user-change-password.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

describe('UserChangePasswordComponent', () => {
  let component: UserChangePasswordComponent;
  let fixture: ComponentFixture<UserChangePasswordComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;
  const user = { id: '123', email: 'email@gmail.com', password: 'oldPassword', firstName: 'name', lastName: 'surname', phone: '1234567890', userName: 'username' };


  beforeEach(async () => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUserId', 'getUserById', 'checkPassword', 'changePassword', 'isLoggedIn', 'isAdmin']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    userServiceSpy.getUserId.and.returnValue('123'); // Asigură-te că returnează ID-ul așteptat
    userServiceSpy.getUserById.and.returnValue(of(user)); // Returnează un Observable cu utilizatorul mock
    userServiceSpy.checkPassword.and.returnValue(of(true)); // Returnează true pentru verificarea parolei
    userServiceSpy.changePassword.and.returnValue(of(true));
    
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, UserChangePasswordComponent],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserChangePasswordComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
    component.ngOnInit();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form on init', () => {
    expect(component.changeForm).toBeDefined();
    expect(component.changeForm.controls['currentPassword']).toBeDefined();
    expect(component.changeForm.controls['newPassword']).toBeDefined();
    expect(component.changeForm.controls['confirmPassword']).toBeDefined();
  });

  it('should reset form and fetch user on init', () => {
    const userId = '123';
    const user = { id: userId, email: 'email@gmail.com', password: 'oldPassword', firstName: 'name', lastName: 'surname', phone: '1234567890', userName: 'username' };
    userService.getUserId.and.returnValue(userId);
    userService.getUserById.and.returnValue(of(user));

    component.ngOnInit();

    expect(component.changeForm.pristine).toBeTrue();
    expect(userService.getUserId).toHaveBeenCalled();
    expect(userService.getUserById).toHaveBeenCalledWith(userId);
    expect(component.user).toEqual(user);
  });

  it('should navigate to profile on successful password change', () => {
    const user = { id: '123', email: 'email@gmail.com', password: 'oldPassword', firstName: 'name', lastName: 'surname', phone: '1234567890', userName: 'username' };
    component.user = user;
    component.changeForm.setValue({
      currentPassword: 'oldPassword',
      newPassword: 'newPassword',
      confirmPassword: 'newPassword'
    });
    userService.checkPassword.and.returnValue(of(true));
    userService.changePassword.and.returnValue(of(true));

    component.onSubmit();

    expect(userService.checkPassword).toHaveBeenCalledWith(user);
    expect(userService.changePassword).toHaveBeenCalledWith(user);
    expect(router.navigate).toHaveBeenCalledWith(['profile']);
  });

  it('should set error message on password check failure', () => {
    const user = { id: '123', email: 'email@gmail.com', password: 'oldPassword', firstName: 'name', lastName: 'surname', phone: '1234567890', userName: 'username' };
    component.user = user;
    component.changeForm.setValue({
      currentPassword: 'wrongPassword',
      newPassword: 'newPassword',
      confirmPassword: 'newPassword'
    });
    userService.checkPassword.and.returnValue(throwError({ error: 'Incorrect password' }));

    component.onSubmit();

    expect(userService.checkPassword).toHaveBeenCalledWith(user);
    expect(component.errorMessage).toBe('Incorrect password');
  });

  it('should set error message on password change failure', () => {
    const user = { id: '123', email: 'email@gmail.com', password: 'oldPassword', firstName: 'name', lastName: 'surname', phone: '1234567890', userName: 'username' };
    component.user = user;
    component.changeForm.setValue({
      currentPassword: 'oldPassword',
      newPassword: 'newPassword',
      confirmPassword: 'newPassword'
    });
    userService.checkPassword.and.returnValue(of(true));
    userService.changePassword.and.returnValue(throwError({ error: 'Change password failed' }));

    component.onSubmit();

    expect(userService.checkPassword).toHaveBeenCalledWith(user);
    expect(userService.changePassword).toHaveBeenCalledWith(user);
    expect(component.errorMessage).toBe('Change password failed');
  });

  it('should navigate to profile on navigateToProfile call', () => {
    component.navigateToProfile();
    expect(router.navigate).toHaveBeenCalledWith(['profile']);
  });
});