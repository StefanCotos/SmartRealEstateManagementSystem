import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { EstateDetailComponent } from './estate-detail.component';
import { EstateService } from '../../services/estate.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { UserService } from '../../services/user.service';
import { ImageService } from '../../services/image.service';
import { FavoriteService } from '../../services/favorite.service';

describe('EstateDetailComponent', () => {
  let component: EstateDetailComponent;
  let fixture: ComponentFixture<EstateDetailComponent>;
  let estateServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let imageServiceMock: any;
  let favoriteServiceMock: any;

  beforeEach(async () => {
    estateServiceMock = {
      getEstateById: jasmine.createSpy('getEstateById').and.returnValue(of({})),
      deleteEstate: jasmine.createSpy('deleteEstate').and.returnValue(of({})),
      buyProduct: jasmine.createSpy('buyProduct').and.returnValue(of('http://example.com')),
      updateEstateProduct: jasmine.createSpy('updateEstateProduct').and.returnValue(of({}))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.callFake((key: string) => {
            if (key === 'id') {
              return '1';
            }
            return null;
          })
        }
      }
    };

    userServiceMock = {
      getUserById: jasmine.createSpy('getUserById').and.returnValue(of({ id: '1', userName: 'testUser' })),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserId: jasmine.createSpy('getUserId').and.returnValue('1'),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('testUser'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
    };

    imageServiceMock = {
      getImagesByEstateId: jasmine.createSpy('getImagesByEstateId').and.returnValue(of([]))
    };

    favoriteServiceMock = {
      getFavoritesByUserId: jasmine.createSpy('getFavoritesByUserId').and.returnValue(of([])),
      deleteFavorite: jasmine.createSpy('deleteFavorite').and.returnValue(of({})),
      saveFavorite: jasmine.createSpy('saveFavorite').and.returnValue(of({}))
    };

    await TestBed.configureTestingModule({
      imports: [EstateDetailComponent],
      providers: [
        { provide: EstateService, useValue: estateServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ImageService, useValue: imageServiceMock },
        { provide: FavoriteService, useValue: favoriteServiceMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EstateDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to edit page on edit', () => {
    component.onEdit('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/update', '1']);
  });

  it('should navigate to reviews page on reviews', () => {
    component.onReviews('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['review-users/get-all', '1']);
  });

  it('should navigate to report page on report', () => {
    component.onReport('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['reports/create', '1']);
  });

  it('should delete estate and navigate to home on delete', () => {
    component.onDelete('1');
    expect(estateServiceMock.deleteEstate).toHaveBeenCalledWith('1');
    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });

  it('should toggle favorite', () => {
    component.toggleFavorite('1');
    expect(favoriteServiceMock.saveFavorite).toHaveBeenCalled();
    component.toggleFavorite('1');
    expect(favoriteServiceMock.deleteFavorite).toHaveBeenCalled();
  });

  it('should return correct favorite icon', () => {
    component.favoriteEstates = '1';
    expect(component.getFavoriteIcon('1')).toEqual('../../../assets/star-filled.svg');
    expect(component.getFavoriteIcon('2')).toEqual('../../../assets/star-outline.svg');
  });

  it('should go to the next slide in the image gallery', () => {
    // Set initial state
    component.images = ['image1', 'image2', 'image3'];
    component.currentSlide = 0;
  
    // Call nextSlide
    component.nextSlide();
  
    // Expect the current slide to move to the next index (1)
    expect(component.currentSlide).toBe(1);
  
    // Call nextSlide again and ensure it loops back to 0
    component.nextSlide();
    expect(component.currentSlide).toBe(2);
  
    component.nextSlide();
    expect(component.currentSlide).toBe(0);
  });
  
  it('should go to the previous slide in the image gallery', () => {
    // Set initial state
    component.images = ['image1', 'image2', 'image3'];
    component.currentSlide = 0;
  
    // Call prevSlide
    component.prevSlide();
  
    // Expect the current slide to go back to the last index (2)
    expect(component.currentSlide).toBe(2);
  
    // Call prevSlide again and ensure it moves to the previous index (1)
    component.prevSlide();
    expect(component.currentSlide).toBe(1);
  
    component.prevSlide();
    expect(component.currentSlide).toBe(0);
  });
  
  it('should call ngOnInit and load estate, user, and images', () => {
    // Mock data for estate and user
    const mockEstate = { id: '1', userId: '1' };
    const mockUser = { id: '1', userName: 'testUser' };
    
    // Mock the calls
    estateServiceMock.getEstateById.and.returnValue(of(mockEstate));
    userServiceMock.getUserById.and.returnValue(of(mockUser));
    imageServiceMock.getImagesByEstateId.and.returnValue(of(['image1', 'image2']));
  
    // Call ngOnInit
    component.ngOnInit();
  
    // Check if getEstateById was called
    expect(estateServiceMock.getEstateById).toHaveBeenCalledWith('1');
  
    // Check if getUserById was called with the correct userId
    expect(userServiceMock.getUserById).toHaveBeenCalledWith('1');
  
    // Check if loadImages was called with the estateId
    expect(imageServiceMock.getImagesByEstateId).toHaveBeenCalledWith('1');
  
    // Ensure the estate and user data are set
    expect(component.estate).toEqual(mockEstate);
    expect(component.username).toBe('testUser');
    expect(component.userId).toBe('1');
  
    // Ensure images are loaded
    expect(component.images.length).toBe(2);
    expect(component.images[0]).toBe(`https://res.cloudinary.com/hlntt5sgz/image/upload/%22undefined%22`);
    expect(component.images[1]).toBe(`https://res.cloudinary.com/hlntt5sgz/image/upload/%22undefined%22`);
  });
  
  it('should load images when they are available', () => {
    const mockImages = ['https://res.cloudinary.com/hlntt5sgz/image/upload/%22undefined%22', 'https://res.cloudinary.com/hlntt5sgz/image/upload/%22undefined%22', 'https://res.cloudinary.com/hlntt5sgz/image/upload/%22undefined%22'];
    imageServiceMock.getImagesByEstateId.and.returnValue(of(mockImages));
  
    // Call loadImages with an estateId
    component.loadImages('1');
  
    // Ensure the images are loaded correctly
    expect(component.images).toEqual(mockImages);
  });
  
  it('should load default image when no images are available', () => {
    imageServiceMock.getImagesByEstateId.and.returnValue(of([]));
  
    // Call loadImages with an estateId
    component.loadImages('1');
  
    // Ensure the images array contains the fallback image
    expect(component.images).toEqual(['https://res.cloudinary.com/hlntt5sgz/image/upload/no_image']);
  });

  it('should check if the estate is in the user\'s favorites', () => {
    const mockEstate = { id: '1', userId: '1' };
    const mockUser = { id: '1', userName: 'testUser' };
    const mockFavorites = [
      { estateId: '1', userId: '1' } // Simulate that estate 1 is in the user's favorites
    ];
  
    // Mock the API calls
    estateServiceMock.getEstateById.and.returnValue(of(mockEstate));
    userServiceMock.getUserById.and.returnValue(of(mockUser));
    favoriteServiceMock.getFavoritesByUserId.and.returnValue(of(mockFavorites));
  
    // Call ngOnInit
    component.ngOnInit();
  
    // Verify that the estate is marked as a favorite
    expect(component.favoriteEstates).toBe('1');
  });
  

});