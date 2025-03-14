import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserFavoritesEstatesComponent } from './user-favorites-estates.component';
import { ActivatedRoute, Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Estate } from '../../models/estate.model';
import { EstateService } from '../../services/estate.service';
import { UserService } from '../../services/user.service';
import { ImageService } from '../../services/image.service';
import { FavoriteService } from '../../services/favorite.service';
import { of, throwError } from 'rxjs';

describe('UserFavoritesEstatesComponent', () => {
  let component: UserFavoritesEstatesComponent;
  let fixture: ComponentFixture<UserFavoritesEstatesComponent>;
  let activatedRouteMock: any;
  let estateServiceMock: any;
  let userServiceMock: any;
  let imageServiceMock: any;
  let favoriteServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    estateServiceMock = {
      getEstatesByIds: jasmine.createSpy('getEstatesByIds').and.returnValue(of([]))
    };

    userServiceMock = {
      getUserId: jasmine.createSpy('getUserId').and.returnValue('1'),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('Test User'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
    };

    imageServiceMock = {
      getImagesByEstateId: jasmine.createSpy('getImagesByEstateId').and.returnValue(of([]))
    };

    favoriteServiceMock = {
      getFavoritesByUserId: jasmine.createSpy('getFavoritesByUserId').and.returnValue(of([]))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [UserFavoritesEstatesComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: EstateService, useValue: estateServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ImageService, useValue: imageServiceMock },
        { provide: FavoriteService, useValue: favoriteServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserFavoritesEstatesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load favorite estates on init', () => {
    expect(userServiceMock.getUserId).toHaveBeenCalled();
    expect(favoriteServiceMock.getFavoritesByUserId).toHaveBeenCalledWith('1');
  });

  it('should update paginated estates', () => {
    component.favoriteEstates = Array(10).fill({}) as Estate[];
    component.updatePaginatedEstates();
    expect(component.paginatedEstates.length).toBe(6);
  });

  it('should change page', () => {
    component.currentPage = 1;
    component.changePage(true);
    expect(component.currentPage).toBe(2);
    component.changePage(false);
    expect(component.currentPage).toBe(1);
  });

  it('should navigate to detail estate', () => {
    component.navigateToDetailEstate('123');
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/detail', '123']);
  });

  it('should return correct pages array', () => {
    component.favoriteEstates = Array(12).fill({}) as Estate[];
    const pagesArray = component.getPagesArray();
    expect(pagesArray.length).toBe(2);
    expect(pagesArray).toEqual([1, 2]);
  });
});