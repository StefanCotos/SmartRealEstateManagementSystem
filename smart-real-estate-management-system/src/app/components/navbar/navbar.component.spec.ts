import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { NavbarComponent } from './navbar.component';
import { User } from '../../models/user.model';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let userService: UserService;
  let router: Router;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    await TestBed.configureTestingModule({
      imports: [NavbarComponent],
      providers: [
        UserService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    userService = TestBed.inject(UserService);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    spyOn(router, 'navigate');
    spyOn(userService, 'logout').and.callThrough();
    spyOn(component, 'getUserName').and.returnValue('Test User');
    spyOn(component, 'isLogged').and.returnValue(true);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should toggle menu expansion', () => {
    component.isMenuExpanded = false;
    component.toggleMenu();
    expect(component.isMenuExpanded).toBeTrue();

    component.toggleMenu();
    expect(component.isMenuExpanded).toBeFalse();
  });

  it('should navigate to login on logout', () => {
    component.logout();
    expect(userService.logout).toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should return the correct username', () => {
    expect(component.getUserName()).toBe('Test User');
  });

  it('should check if user is logged in', () => {
    expect(component.isLogged()).toBeTrue();
  });

  it('should navigate to estate list', () => {
    component.navigateToEstateList();
    expect(router.navigate).toHaveBeenCalledWith(['']);
  });

  it('should navigate to create estate', () => {
    component.navigateToCreateEstate();
    expect(router.navigate).toHaveBeenCalledWith(['estates/create']);
  });

  it('should navigate to estimate price', () => {
    component.navigateToEstimatePrice();
    expect(router.navigate).toHaveBeenCalledWith(['estates/estimate-price']);
  });

  it('should navigate to register', () => {
    component.navigateToRegister();
    expect(router.navigate).toHaveBeenCalledWith(['register']);
  });

  it('should navigate to profile', () => {
    component.navigateToProfile();
    expect(router.navigate).toHaveBeenCalledWith(['profile']);
  });

  it('should navigate to favorite estates', () => {
    component.navigateToFavoriteEstates();
    expect(router.navigate).toHaveBeenCalledWith(['users/favorites']);
  });

  it('should navigate to login', () => {
    component.navigateToLogin();
    expect(router.navigate).toHaveBeenCalledWith(['login']);
  });

  it('should navigate to contact', () => {
    component.navigateToContact();
    expect(router.navigate).toHaveBeenCalledWith(['contact']);
  });

  it('should navigate to admin page', () => {
    component.navigateToAdminPage();
    expect(router.navigate).toHaveBeenCalledWith(['admin']);
  });

  // Integration test for NavbarComponent
  it('should display the correct menu state in the template', () => {
    component.isMenuExpanded = true;
    fixture.detectChanges();
    let compiled = fixture.nativeElement;
    expect(compiled.querySelector('.menu .content')).toBeTruthy();

    component.isMenuExpanded = false;
    fixture.detectChanges();
    compiled = fixture.nativeElement;
    expect(compiled.querySelector('.menu .content')).toBeNull();
  });
});
