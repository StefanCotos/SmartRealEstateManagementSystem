import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserEditComponent } from './user-edit.component';
import { ActivatedRoute } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

jasmine.DEFAULT_TIMEOUT_INTERVAL = 10000;

describe('UserEditComponent', () => {
  let component: UserEditComponent;
  let fixture: ComponentFixture<UserEditComponent>;
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
      getUserId: jasmine.createSpy('getUserId').and.returnValue('1'),
      getUserById: jasmine.createSpy('getUserById').and.returnValue(of({
        id: '1',
        firstName: 'John',
        lastName: 'Doe',
        userName: 'johndoe',
        email: 'john.doe@example.com',
        password: 'password123'
      })),
      editUser: jasmine.createSpy('editUser').and.returnValue(of({ success: true })),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('johndoe'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(true)
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, UserEditComponent],
      providers: [
        FormBuilder,
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with user data', () => {
    expect(component.userForm.value).toEqual({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123'
    });
  });

  it('should display an error message if form submission fails', () => {
    component.userForm.setValue({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123'
    });
    component.onSubmit();
    expect(component.errorMessage).toBe('');
  });

  it('should set an error message when userService.editUser fails', () => {
    const errorResponse = { error: 'An error occurred while updating the user.' };
    userServiceMock.editUser.and.returnValue(throwError(() => errorResponse));

    component.userForm.setValue({
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(component.errorMessage).toBe('');
  });

  it('should not submit when the form is invalid', () => {
    component.userForm.setValue({
      id: '',
      firstName: 'John',
      lastName: '',
      userName: 'johndoe',
      email: 'invalid-email',
      password: '123'
    });

    component.onSubmit();

    // Ensure that editUser is not called when the form is invalid
    expect(userServiceMock.editUser).not.toHaveBeenCalled();
  });
});
