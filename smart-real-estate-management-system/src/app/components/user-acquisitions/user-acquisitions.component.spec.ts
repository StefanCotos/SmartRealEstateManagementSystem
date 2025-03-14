import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserAcquisitionsComponent } from './user-acquisitions.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { EstateService } from '../../services/estate.service';
import { UserService } from '../../services/user.service';
import { ImageService } from '../../services/image.service';
import { Router } from '@angular/router';

describe('UserAcquisitionsComponent', () => {
  let component: UserAcquisitionsComponent;
  let fixture: ComponentFixture<UserAcquisitionsComponent>;
  let estateService: jasmine.SpyObj<EstateService>;
  let userService: jasmine.SpyObj<UserService>;
  let imageService: jasmine.SpyObj<ImageService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const estateServiceSpy = jasmine.createSpyObj('EstateService', ['getEstatesByBuyerId']);
    estateServiceSpy.getEstatesByBuyerId.and.returnValue(of([{ id: '1' }, { id: '2' }]));
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUserId', 'isLoggedIn', 'getUserName', 'isAdmin']);
    userServiceSpy.getUserId.and.returnValue(of('userId'));
    const imageServiceSpy = jasmine.createSpyObj('ImageService', ['getImagesByEstateId']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    imageServiceSpy.getImagesByEstateId.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [UserAcquisitionsComponent],
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

    fixture = TestBed.createComponent(UserAcquisitionsComponent);
    component = fixture.componentInstance;
    estateService = TestBed.inject(EstateService) as jasmine.SpyObj<EstateService>;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    userService.isLoggedIn.and.returnValue(true);
    imageService = TestBed.inject(ImageService) as jasmine.SpyObj<ImageService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load user estates on init', () => {
    const estatesMock = [{ id: '1' }, { id: '2' }] as any;
    userService.getUserId.and.returnValue('userId');
    estateService.getEstatesByBuyerId.and.returnValue(of(estatesMock));

    component.ngOnInit();

    expect(userService.getUserId).toHaveBeenCalled();
    expect(estateService.getEstatesByBuyerId).toHaveBeenCalledWith('userId');
    expect(component.estates).toEqual(estatesMock);
  });

  it('should update paginated estates', () => {
    component.estates = Array(10).fill({ id: '1' }) as any;
    component.currentPage = 1;
    component.itemsPerPage = 5;

    component.updatePaginatedEstates();

    expect(component.paginatedEstates.length).toBe(5);
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

  it('should return correct pages array', () => {
    component.estates = Array(12).fill({}) as any;
    component.itemsPerPage = 5;

    const pagesArray = component.getPagesArray();

    expect(pagesArray.length).toBe(3);
  });
});