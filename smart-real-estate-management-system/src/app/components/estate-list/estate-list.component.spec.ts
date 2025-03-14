import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EstateListComponent } from './estate-list.component';
import { EstateService } from '../../services/estate.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { FavoriteService } from '../../services/favorite.service';

describe('EstateListComponent', () => {
  let component: EstateListComponent;
  let fixture: ComponentFixture<EstateListComponent>;
  let estateServiceMock: any;
  let favoriteServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    estateServiceMock = {
      getPaginatedEstates: jasmine.createSpy('getPaginatedEstates').and.returnValue(of({
        data: {
          data: [
            { id: '1', name: 'Estate 1', address: 'Address 1', price: 100, size: 50, listingData: '2024-01-01' },
            { id: '2', name: 'Estate 2', address: 'Address 2', price: 200, size: 100, listingData: '2024-01-02' }
          ],
          totalCount: 6
        }
      }))
    };

    favoriteServiceMock = {
      getFavoritesByUserId: jasmine.createSpy('getFavoritesByUserId').and.returnValue(of([{ estateId: '1', userId: '1' }])
      ),
      saveFavorite: jasmine.createSpy('saveFavorite').and.returnValue(of({ estateId: '1', userId: '1' })),
      deleteFavorite: jasmine.createSpy('deleteFavorite').and.returnValue(of({ estateId: '1', userId: '1' }))
    };

    // Mock Router
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule, EstateListComponent],
      providers: [
        { provide: EstateService, useValue: estateServiceMock },
        { provide: FavoriteService, useValue: favoriteServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EstateListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load estates on initialization', () => {
    component.pageSize = 4;
    component.ngOnInit();
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 1, pageSize: 4 }));
    expect(component.estates.length).toBe(2);
    expect(component.totalPages).toBe(2);
  });

  it('should navigate to the create estate page', () => {
    component.navigateToCreateEstate();
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/create']);
  });

  it('should navigate to the update estate page', () => {
    const id = '123';
    component.navigateToUpdateEstate(id);
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/update', id]);
  });

  it('should navigate to the detail estate page', () => {
    const id = '123';
    component.navigateToDetailEstate(id);
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/detail', id]);
  });

  it('should change page and load estates', () => {
    component.currentPage = 1;
    component.pageSize = 4;
    component.totalPages = 3;
    component.changePage(true);
    expect(component.currentPage).toBe(2);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 2, pageSize: 4 }));
  });

  it('should not change page if on the first page and "previous" is clicked', () => {
    component.currentPage = 1;
    component.pageSize = 4;
    component.changePage(false);
    expect(component.currentPage).toBe(1);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 1, pageSize: 6 }));
  });

  it('should change page size and reset to the first page', () => {
    const event = { target: { value: '5' } } as unknown as Event;
    component.changePageSize(event);
    expect(component.pageSize).toBe(5);
    expect(component.currentPage).toBe(1);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 1, pageSize: 5 }));
  });

  it('should go to a specific page and load estates', () => {
    component.pageSize = 4;
    component.goToPage(3);
    expect(component.currentPage).toBe(3);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 3, pageSize: 4 }));
  });

  it('should return the correct pages array', () => {
    component.totalPages = 3;
    expect(component.getPagesArray()).toEqual([1, 2, 3]);
  });

  it('should not change page if on the last page and "next" is clicked', () => {
    component.currentPage = 3;
    component.pageSize = 4;
    component.totalPages = 3;
    component.changePage(true);
    expect(component.currentPage).toBe(3);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 3, pageSize: 4 }));
  });

  it('should change to the next page if not on the last page and "next" is clicked', () => {
    component.currentPage = 2;
    component.pageSize = 4;
    component.totalPages = 3;
    component.changePage(true);
    expect(component.currentPage).toBe(3);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: '', street: '', city: '', state: '', price: 0, bedrooms: 0, bathrooms: 0, landSize: 0, houseSize: 0 }), Object({ page: 3, pageSize: 4 }));
  });

  it('should change to the previous page if not on the first page and "previous" is clicked', () => {
    component.filter.name = 'Estate 1';
    component.filter.bedrooms = 2;
    component.filter.bathrooms = 2;
    component.filter.landSize = 100;
    component.filter.street = 'A';
    component.filter.city = 'B';
    component.filter.state = 'C';
    component.filter.price = 100;
    component.currentPage = 2;
    component.pageSize = 4;
    component.totalPages = 3;
    component.changePage(false);
    expect(component.currentPage).toBe(1);
    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(Object({ name: 'Estate 1', street: 'A', city: 'B', state: 'C', price: 100, bedrooms: 2, bathrooms: 2, landSize: 100, houseSize: 0 }), Object({ page: 1, pageSize: 4 }));

  });

  it('should load estates and set image cache correctly', () => {
    component.pageSize = 4;
    component.ngOnInit(); // Calls loadEstates

    expect(estateServiceMock.getPaginatedEstates).toHaveBeenCalledWith(
      {
        name: '',
        street: '',
        city: '',
        state: '',
        price: 0,
        bedrooms: 0,
        bathrooms: 0,
        landSize: 0,
        houseSize: 0
      },
      {
        page: 1,
        pageSize: 4
      }
    );

    // Check if the imageCache is updated correctly for each estate
    expect(component.imageCache['1']).not.toBeDefined();
    expect(component.imageCache['2']).not.toBeDefined();
    expect(component.estates.length).toBe(2);
    expect(component.totalPages).toBe(2);
  });

  it('should toggle favorite correctly', () => {
    const estateId = '1';
    const userId = '';

    // Initially, the estate is not favorited
    component.favoriteEstates = new Set();


    // Add to favorites
    component.toggleFavorite(estateId);
    expect(component.favoriteService.saveFavorite).toHaveBeenCalledWith({ userId, estateId });
    expect(component.favoriteEstates.has(estateId)).toBeTrue();

    // Remove from favorites
    component.toggleFavorite(estateId);
    expect(component.favoriteService.deleteFavorite).toHaveBeenCalledWith(userId, estateId);
    expect(component.favoriteEstates.has(estateId)).toBeFalse();
  });

  it('should return correct favorite icon', () => {
    const estateId = '1';
  
    // When the estate is favorited
    component.favoriteEstates.add(estateId);
    let icon = component.getFavoriteIcon(estateId);
    expect(icon).toBe('../../../assets/star-filled.svg');
  
    // When the estate is not favorited
    component.favoriteEstates.delete(estateId);
    icon = component.getFavoriteIcon(estateId);
    expect(icon).toBe('../../../assets/star-outline.svg');
  });
  
  it('should initialize collapsible filters correctly', () => {
    // Arrange: Create a mock for the collapsible element
    const mockCollapsibleElement = {
      addEventListener: jasmine.createSpy('addEventListener')
    };
    
    // Simulate document.getElementsByClassName to return the mock element
    spyOn(document, 'getElementsByClassName').and.returnValue([mockCollapsibleElement] as any);
  
    // Act: Initialize the collapsible elements
    component.initializeCollapsible();
  
    // Assert: Check that the event listener was added
    expect(mockCollapsibleElement.addEventListener).toHaveBeenCalledWith('click', jasmine.any(Function));
  });

  it('should apply filters and load estates from page 1', () => {
    // Arrange: Set initial filters and page state
    component.filter.name = 'Estate 1';
    component.filter.city = 'City A';
    component.currentPage = 5; // Simulating that we are on page 5
  
    // Spy on loadEstates to check if it's called with correct parameters
    spyOn(component, 'loadEstates');
  
    // Act: Call applyFilter
    component.applyFilter();
  
    // Assert: Check that currentPage is reset to 1
    expect(component.currentPage).toBe(1);
  
    // Assert: Check that loadEstates is called
    expect(component.loadEstates).toHaveBeenCalled();
    
    // Optionally: Check that loadEstates is called with the correct filters
    expect(component.loadEstates).toHaveBeenCalledWith();
  });
  
  it('should call loadEstates and initializeCollapsible on ngOnInit', () => {
    spyOn(component, 'loadEstates');
    spyOn(component, 'initializeCollapsible');

    component.ngOnInit();

    expect(component.loadEstates).toHaveBeenCalled();
    expect(component.initializeCollapsible).toHaveBeenCalled();
  });

  it('should navigate to home page when navigateToHome is called', () => {
    component.navigateToHome();
    
    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });

});