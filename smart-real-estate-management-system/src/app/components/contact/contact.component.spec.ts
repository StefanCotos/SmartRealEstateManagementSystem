import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactComponent } from './contact.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { UserService } from '../../services/user.service';
import { of, throwError } from 'rxjs';

describe('ContactComponent', () => {
  let component: ContactComponent;
  let fixture: ComponentFixture<ContactComponent>;
  let userService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);
    userService = jasmine.createSpyObj('UserService', ['sendContactForm', 'isLoggedIn', 'isAdmin']);

    await TestBed.configureTestingModule({
      imports: [ContactComponent, ReactiveFormsModule],
      providers: [
        { provide: UserService, useValue: userService },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(ContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a form with 4 controls', () => {
    expect(component.contactForm.contains('name')).toBeTruthy();
    expect(component.contactForm.contains('email')).toBeTruthy();
    expect(component.contactForm.contains('subject')).toBeTruthy();
    expect(component.contactForm.contains('message')).toBeTruthy();
  });

  it('should make the name control required', () => {
    let control = component.contactForm.get('name');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should make the email control required and validate email format', () => {
    let control = component.contactForm.get('email');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();

    control?.setValue('invalid-email');
    expect(control?.valid).toBeFalsy();

    control?.setValue('test@example.com');
    expect(control?.valid).toBeTruthy();
  });

  it('should make the subject control required', () => {
    let control = component.contactForm.get('subject');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should make the message control required', () => {
    let control = component.contactForm.get('message');
    control?.setValue('');
    expect(control?.valid).toBeFalsy();
  });

  it('should call sendContactForm on valid form submission and reset the form on success', () => {
    // Arrange
    component.contactForm.setValue({
      name: 'John Doe',
      email: 'john.doe@example.com',
      subject: 'Test Subject',
      message: 'Test message',
    });

    userService.sendContactForm.and.returnValue(of({ success: true }));

    spyOn(component.contactForm, 'reset'); // Spy on the reset method

    // Act
    component.onSubmit();

    // Assert
    expect(userService.sendContactForm).toHaveBeenCalledWith(component.contactForm.value);
    expect(component.contactForm.reset).toHaveBeenCalled();
    expect(component.errorMessage).toBe('');
  });

  it('should set errorMessage on failed form submission', () => {
    // Arrange
    component.contactForm.setValue({
      name: 'John Doe',
      email: 'john.doe@example.com',
      subject: 'Test Subject',
      message: 'Test message',
    });

    userService.sendContactForm.and.returnValue(
      throwError(() => ({
        error: 'An error occurred while sending the form',
      }))
    );

    // Act
    component.onSubmit();

    // Assert
    expect(userService.sendContactForm).toHaveBeenCalledWith(component.contactForm.value);
    expect(component.errorMessage).toBe('An error occurred while sending the form');
  });
});