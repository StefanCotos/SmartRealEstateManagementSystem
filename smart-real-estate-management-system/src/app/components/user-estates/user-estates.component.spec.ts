import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserEstatesComponent } from './user-estates.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { EstateService } from '../../services/estate.service';
import { UserService } from '../../services/user.service';
import { ImageService } from '../../services/image.service';
import { Router } from '@angular/router';

describe('UserEstatesComponent', () => {
  let component: UserEstatesComponent;
  let fixture: ComponentFixture<UserEstatesComponent>;
  let estateService: jasmine.SpyObj<EstateService>;
  let userService: jasmine.SpyObj<UserService>;
  let imageService: jasmine.SpyObj<ImageService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const estateServiceSpy = jasmine.createSpyObj('EstateService', { getEstatesByUserId: of([]) });
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUserId', 'isLoggedIn', 'isAdmin']);
    const imageServiceSpy = jasmine.createSpyObj('ImageService', ['getImagesByEstateId']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    imageServiceSpy.getImagesByEstateId.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [UserEstatesComponent],
      providers: [
        { provide: EstateService, useValue: estateServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: ImageService, useValue: imageServiceSpy },
        { provide: Router, useValue: routerSpy },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserEstatesComponent);
    component = fixture.componentInstance;
    estateService = TestBed.inject(EstateService) as jasmine.SpyObj<EstateService>;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    imageService = TestBed.inject(ImageService) as jasmine.SpyObj<ImageService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load user estates on init', () => {
    const mockEstates = [
      {
        id: '1',
        userId: 'userId',
        name: 'Estate 1',
        description: 'Description 1',
        price: 100,
        bedrooms: 3,
        bathrooms: 2,
        landSize: 500,
        street: 'Street 1',
        city: 'City 1',
        state: 'State 1',
        zipCode: '12345',
        houseSize: 150,
        listingData: new Date(),
      },

      {
        id: '2',
        userId: 'userId',
        name: 'Estate 2',
        description: 'Description 2',
        price: 200,
        bedrooms: 4,
        bathrooms: 3,
        landSize: 600,
        street: 'Street 2',
        city: 'City 2',
        state: 'State 2',
        zipCode: '67890',
        houseSize: 200,
        listingData: new Date(),
      },

    ];
    userService.getUserId.and.returnValue('userId');
    estateService.getEstatesByUserId.and.returnValue(of(mockEstates));

    component.ngOnInit();

    expect(userService.getUserId).toHaveBeenCalled();
    expect(estateService.getEstatesByUserId).toHaveBeenCalledWith('userId');
    expect(component.estates).toEqual(mockEstates);
  });

  it('should update paginated estates', () => {
    component.estates = Array.from({ length: 10 }, (_, i) => ({
      id: `${i + 1}`,
      userId: 'userId',
      name: `Estate ${i + 1}`,
      description: `Description ${i + 1}`,
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: `Street ${i + 1}`,
      city: `City ${i + 1}`,
      state: `State ${i + 1}`,
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date(),
    }));
    component.currentPage = 1;
    component.itemsPerPage = 5;

    component.updatePaginatedEstates();

    expect(component.paginatedEstates.length).toBe(5);
    expect(component.paginatedEstates[0].id).toBe('1');
    expect(component.paginatedEstates[4].id).toBe('5');
  });

  it('should navigate to detail estate', () => {
    const estateId = '1';
    component.navigateToDetailEstate(estateId);

    expect(router.navigate).toHaveBeenCalledWith(['estates/detail', estateId]);
  });

  it('should change page', () => {
    component.currentPage = 1;
    component.changePage(true);
    expect(component.currentPage).toBe(2);

    component.changePage(false);
    expect(component.currentPage).toBe(1);
  });

  it('should return pages array', () => {
    component.estates = Array.from({ length: 12 }, (_, i) => ({
      id: `${i + 1}`,
      userId: 'userId',
      name: `Estate ${i + 1}`,
      description: `Description ${i + 1}`,
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: `Street ${i + 1}`,
      city: `City ${i + 1}`,
      state: `State ${i + 1}`,
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date(),
    }));
    component.itemsPerPage = 5;

    const pagesArray = component.getPagesArray();

    expect(pagesArray.length).toBe(3);
    expect(pagesArray).toEqual([1, 2, 3]);
  });
});