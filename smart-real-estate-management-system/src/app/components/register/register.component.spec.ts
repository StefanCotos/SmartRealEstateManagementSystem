import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterComponent } from './register.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { UserService } from '../../services/user.service';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['register', 'isLoggedIn', 'isAdmin']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [RegisterComponent, ReactiveFormsModule],
      providers: [
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a valid form when all fields are filled correctly', () => {
    component.registerForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123',
      confirmPassword: 'password123'
    });
    expect(component.registerForm.valid).toBeTrue();
  });

  it('should have an invalid form when fields are empty', () => {
    component.registerForm.setValue({
      firstName: '',
      lastName: '',
      userName: '',
      email: '',
      password: '',
      confirmPassword: ''
    });
    expect(component.registerForm.invalid).toBeTrue();
  });

  it('should call userService.register and navigate to login on successful registration', () => {
    component.registerForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123',
      confirmPassword: 'password123'
    });
    userService.register.and.returnValue(of({
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123'
    }));

    component.onSubmit();

    expect(userService.register).toHaveBeenCalledWith(component.registerForm.value);
    expect(router.navigate).toHaveBeenCalledWith(['/confirm-email']);
  });

  it('should set errorMessage on registration failure', () => {
    component.registerForm.setValue({
      firstName: 'John',
      lastName: 'Doe',
      userName: 'johndoe',
      email: 'john.doe@example.com',
      password: 'password123',
      confirmPassword: 'password123'
    });
    const errorResponse = { error: 'Registration failed' };
    userService.register.and.returnValue(throwError(errorResponse));

    component.onSubmit();

    expect(component.errorMessage).toBe('Registration failed');
  });
});