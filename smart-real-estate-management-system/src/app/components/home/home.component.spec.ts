import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeComponent } from './home.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { provideRouter } from '@angular/router';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let router: Router;
  let userService: UserService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
        {
          provide: UserService,
          useValue: {
            isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
            getUserId: jasmine.createSpy('getUserId').and.returnValue('123'),
            getUserName: jasmine.createSpy('getUserName').and.returnValue('John Doe'),
            isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
          }
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    userService = TestBed.inject(UserService);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to create estate', () => {
    const navigateSpy = spyOn(router, 'navigate');
    component.navigateToCreateEstate();
    expect(navigateSpy).toHaveBeenCalledWith(['estates/create']);
  });

  it('should return true if user is logged in', () => {
    expect(component.isLogged()).toBeTrue();
  });

  it('should return false if user is not logged in', () => {
    (userService.isLoggedIn as jasmine.Spy).and.returnValue(false);
    expect(component.isLogged()).toBeFalse();
  });
});
