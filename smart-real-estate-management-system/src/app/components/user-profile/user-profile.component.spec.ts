import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserProfileComponent } from './user-profile.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { of, throwError } from 'rxjs';

describe('UserProfileComponent', () => {
  let component: UserProfileComponent;
  let fixture: ComponentFixture<UserProfileComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    userServiceMock = {
      getUserById: jasmine.createSpy('getUserById').and.returnValue(of({ id: 1, name: 'John Doe' })),
      getUserId: jasmine.createSpy('getUserId').and.returnValue('1'),
      checkPassword: jasmine.createSpy('checkPassword').and.returnValue(of(true)),
      removeUser: jasmine.createSpy('removeUser').and.returnValue(of({})),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue(of('John Doe')),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(of(true))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [UserProfileComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load user data on init', () => {
    expect(userServiceMock.getUserById).toHaveBeenCalledWith('1');
    expect(component.user).toEqual({ id: 1, name: 'John Doe' });
  });

  it('should navigate to edit profile', () => {
    component.editProfile();
    expect(routerMock.navigate).toHaveBeenCalledWith(['users/edit']);
  });

  it('should navigate to see reviews', () => {
    component.seeReviews('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['review-users/get-all', '1']);
  });

  it('should navigate to see estates', () => {
    component.seeEstates();
    expect(routerMock.navigate).toHaveBeenCalledWith(['users/estates-list']);
  });

  it('should navigate to see acquisitions', () => {
    component.seeAcquisitions();
    expect(routerMock.navigate).toHaveBeenCalledWith(['users/acquisitions']);
  });

  it('should navigate to change password', () => {
    component.changePassword();
    expect(routerMock.navigate).toHaveBeenCalledWith(['users/change-password']);
  });

  it('should remove profile if password is correct', () => {
    spyOn(window, 'prompt').and.returnValue('password');
    spyOn(window, 'confirm').and.returnValue(true);

    component.removeProfile();

    expect(userServiceMock.checkPassword).toHaveBeenCalled();
    expect(userServiceMock.removeUser).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });

  it('should not remove profile if password is incorrect', () => {
    spyOn(window, 'prompt').and.returnValue('password');
    userServiceMock.checkPassword.and.returnValue(of(false));

    component.removeProfile();

    expect(userServiceMock.checkPassword).toHaveBeenCalled();
    expect(userServiceMock.removeUser).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should show error if checkPassword fails', () => {
    spyOn(window, 'prompt').and.returnValue('password');
    userServiceMock.checkPassword.and.returnValue(throwError({ error: 'Invalid password' }));
    spyOn(window, 'alert');

    component.removeProfile();

    expect(userServiceMock.checkPassword).toHaveBeenCalled();
    expect(window.alert).toHaveBeenCalledWith('Invalid password');
  });

  it('should not remove profile if password prompt is cancelled', () => {
    spyOn(window, 'prompt').and.returnValue(null);

    component.removeProfile();

    expect(userServiceMock.checkPassword).not.toHaveBeenCalled();
    expect(userServiceMock.removeUser).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should show alert if password is not provided', () => {
    spyOn(window, 'prompt').and.returnValue('');
    spyOn(window, 'alert');

    component.removeProfile();

    expect(window.alert).toHaveBeenCalledWith('Password is required to remove the profile.');
    expect(userServiceMock.checkPassword).not.toHaveBeenCalled();
    expect(userServiceMock.removeUser).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should not remove profile if user cancels confirmation', () => {
    spyOn(window, 'prompt').and.returnValue('password');
    spyOn(window, 'confirm').and.returnValue(false);

    component.removeProfile();

    expect(userServiceMock.checkPassword).toHaveBeenCalled();
    expect(userServiceMock.removeUser).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });
});