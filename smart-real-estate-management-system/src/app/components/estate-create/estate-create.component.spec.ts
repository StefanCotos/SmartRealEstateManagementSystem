import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EstateCreateComponent } from './estate-create.component';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { EstateService } from '../../services/estate.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { CloudinaryService } from '../../services/cloudinary.service';
import { ImageService } from '../../services/image.service';
import { UserService } from '../../services/user.service';

describe('EstateCreateComponent', () => {
  let component: EstateCreateComponent;
  let fixture: ComponentFixture<EstateCreateComponent>; 
  let estateServiceMock: any;
  let cloudinaryServiceMock: any;
  let imageServiceMock: any;
  let userServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    estateServiceMock = jasmine.createSpyObj('EstateService', [
      'createEstate',
      'createProductStripe',
      'updateEstate',
      'estimateEstatePrice',
    ]);
    estateServiceMock.createEstate.and.returnValue(of('valid-estate-id'));
    estateServiceMock.createProductStripe.and.returnValue(of({ priceId: 'mock-price-id', productId: 'mock-product-id' }));
    estateServiceMock.updateEstate.and.returnValue(of(null));
    estateServiceMock.estimateEstatePrice.and.returnValue(of(1000));

    cloudinaryServiceMock = jasmine.createSpyObj('CloudinaryService', ['uploadFile']);

    imageServiceMock = jasmine.createSpyObj('ImageService', ['saveImage']);
    imageServiceMock.saveImage.and.returnValue(of('mock-image-id'));

    userServiceMock = jasmine.createSpyObj('UserService', ['getUserId', 'isLoggedIn', 'isAdmin', 'getUserName']);
    userServiceMock.isLoggedIn.and.returnValue(true);
    userServiceMock.getUserId.and.returnValue('mock-user-id');
    userServiceMock.isAdmin.and.returnValue(true);
    userServiceMock.getUserName.and.returnValue('mock-user-name');
    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };
  
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, EstateCreateComponent],
      providers: [
        { provide: EstateService, useValue: estateServiceMock },
        { provide: CloudinaryService, useValue: cloudinaryServiceMock },
        { provide: ImageService, useValue: imageServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),  
        provideHttpClientTesting()
      ]
    }).compileComponents();
  
    fixture = TestBed.createComponent(EstateCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have a valid form when all fields are filled correctly', () => {
    component.estateForm.setValue({
      id: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate',
      description: 'Test Description',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
    });
    expect(component.estateForm.valid).toBeTrue();
  });

  it('should have an invalid form when required fields are missing', () => {
    component.estateForm.setValue({
      id: '',
      name: '',
      description: '',
      price: '',
      bedrooms: '',
      bathrooms: '',
      landSize: '',
      street: '',
      city: '',
      state: '',
      zipCode: '',
      houseSize: '',
    });
    expect(component.estateForm.invalid).toBeTrue();
  });

  it('should not call createEstate on invalid form submission', () => {
    component.estateForm.setValue({
      id: '',
      name: '',
      description: '',
      price: '',
      bedrooms: '',
      bathrooms: '',
      landSize: '',
      street: '',
      city: '',
      state: '',
      zipCode: '',
      houseSize: '',
    });

    component.onSubmit();

    expect(estateServiceMock.createEstate).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should estimate price when estimatePrice is called', () => {
    component.estateForm.setValue({
      id: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate',
      description: 'Test Description',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
    });

    component.estimatePrice();

    expect(estateServiceMock.estimateEstatePrice).toHaveBeenCalled();
    expect(component.price).toBe(1000);
  });

  it('should copy selected fields to estimate form', () => {
    component.estateForm.setValue({
      id: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate',
      description: 'Test Description',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
    });

    component.copySelectedFieldsToEstimateForm();

    expect(component.estimatePriceForm.value).toEqual({
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
    });
  });

  describe('onSubmit', () => {
    it('should call createEstate and navigate on valid form submission', () => {
      component.estateForm.setValue({
        id: '',
        name: 'Test Estate',
        description: 'A beautiful estate',
        price: 500000,
        bedrooms: 4,
        bathrooms: 2,
        landSize: 2000,
        street: 'Test Street',
        city: 'Test City',
        state: 'Test State',
        zipCode: '12345',
        houseSize: 350,
      });
    
      component.onSubmit();
    
      expect(estateServiceMock.createEstate).toHaveBeenCalled();
      expect(routerMock.navigate).toHaveBeenCalledWith(['']);
    });
    

    it('should not call createEstate or navigate on invalid form submission', () => {
      component.estateForm.setValue({
        id: '',
        name: '',
        description: '',
        price: '',
        bedrooms: '',
        bathrooms: '',
        landSize: '',
        street: '',
        city: '',
        state: '',
        zipCode: '',
        houseSize: '',
      });

      component.onSubmit();

      expect(estateServiceMock.createEstate).not.toHaveBeenCalled();
      expect(routerMock.navigate).not.toHaveBeenCalled();
    });  
  });

  describe('onFileSelected', () => {
    it('should update files and selectedFilesCount when files are selected', () => {
      const file1 = new File(['content1'], 'image1.jpg', { type: 'image/jpeg' });
      const file2 = new File(['content2'], 'image2.jpg', { type: 'image/jpeg' });
      const event = { target: { files: [file1, file2] } } as unknown as Event;

      component.onFileSelected(event);

      expect(component.files).toEqual([file1, file2]);
      expect(component.selectedFilesCount).toBe(2);
    });

    it('should reset files and selectedFilesCount when no files are selected', () => {
      const event = { target: { files: [] } } as unknown as Event;

      component.onFileSelected(event);

      expect(component.files).toEqual([]);
      expect(component.selectedFilesCount).toBe(0);
    });
  });
});
