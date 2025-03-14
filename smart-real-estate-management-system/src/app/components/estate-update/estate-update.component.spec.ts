
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { EstateService } from '../../services/estate.service';
import { EstateUpdateComponent } from './estate-update.component';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { CloudinaryService } from '../../services/cloudinary.service';
import { ImageService } from '../../services/image.service';
import { UserService } from '../../services/user.service';
import { Image } from '../../models/image.model';

describe('EstateUpdateComponent', () => {
  let component: EstateUpdateComponent;
  let fixture: ComponentFixture<EstateUpdateComponent>;
  let estateServiceMock: any;
  let cloudinaryServiceMock: any;
  let imageServiceMock: any;
  let userServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    estateServiceMock = {
      getEstateById: jasmine.createSpy('getEstateById').and.returnValue(of({})),
      updateEstate: jasmine.createSpy('updateEstate').and.returnValue(of({})),
      updateEstateProduct: jasmine.createSpy('updateEstateProduct').and.returnValue(of({}))
    };

    imageServiceMock = {
      saveImage: jasmine.createSpy('saveImage').and.returnValue(of({}))
    };

    cloudinaryServiceMock = {
      uploadFile: jasmine.createSpy('uploadFile')
    };

    userServiceMock = {
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('John Doe')
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, EstateUpdateComponent], // Importă componenta aici
      providers: [
        { provide: EstateService, useValue: estateServiceMock },
        { provide: CloudinaryService, useValue: cloudinaryServiceMock },
        { provide: ImageService, useValue: imageServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EstateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form with empty values', () => {
    const formValues = component.estateForm.value;
    expect(formValues.userId).toBe('');
    expect(formValues.name).toBe('');
    expect(formValues.description).toBe('');
    expect(formValues.price).toBe('');
    expect(formValues.bedrooms).toBe('');
    expect(formValues.bathrooms).toBe('');
    expect(formValues.landSize).toBe('');
    expect(formValues.street).toBe('');
    expect(formValues.city).toBe('');
    expect(formValues.state).toBe('');
    expect(formValues.zipCode).toBe('');
    expect(formValues.houseSize).toBe('');
    expect(formValues.id).toBe('');
  });

  it('should call getEstateById on init if estateId is present', () => {
    expect(estateServiceMock.getEstateById).toHaveBeenCalledWith('1');
  });

  it('should patch form values when getEstateById returns data', () => {
    const estateData = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
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
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };
    estateServiceMock.getEstateById.and.returnValue(of(estateData));
    component.ngOnInit();
    expect(component.estateForm.value).toEqual(estateData);
  });

  it('should not call updateEstate if form is invalid', () => {
    component.estateForm.setValue({
      userId: '',
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
      id: ''
    });
    component.onSubmit();
    expect(estateServiceMock.updateEstate).not.toHaveBeenCalled();
    expect(routerMock.navigate).not.toHaveBeenCalled();
  });

  it('should call updateEstate and navigate to estate detail on submit when form is valid', () => {
    const formValue = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
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
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53',
    };

    component.estateForm.setValue(formValue);

    estateServiceMock.getEstateById.and.returnValue(of({ productId: 'prod123', priceId: 'price123' }));
    component.onSubmit();

    expect(estateServiceMock.updateEstate).toHaveBeenCalledWith({
      ...formValue,
      listingData: jasmine.any(String),
      productId: undefined,
      priceId: undefined,
      unitAmount: 100,
      zBANIpierduti: undefined
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['estates/detail', formValue.id]);
  });

  it('should call uploadFiles after updating estate', () => {
    const formValue = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
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
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    component.estateForm.setValue(formValue);
    estateServiceMock.getEstateById.and.returnValue(of({ productId: 'prod123', priceId: 'price123' }));
    
    const files = [new File([''], 'test.jpg')];
    component.files = files;

    spyOn(component, 'uploadFiles').and.callThrough();
    component.onSubmit();

    expect(imageServiceMock.saveImage).toHaveBeenCalled();
    expect(cloudinaryServiceMock.uploadFile).toHaveBeenCalled();
  });

  it('should update selected files and count when files are selected', () => {
    const event = {
      target: {
        files: [new File([''], 'test.jpg'), new File([''], 'test2.jpg')]
      }
    };

    component.onFileSelected(event as any);

    expect(component.selectedFilesCount).toBe(2);
    expect(component.files.length).toBe(2);
  });
});