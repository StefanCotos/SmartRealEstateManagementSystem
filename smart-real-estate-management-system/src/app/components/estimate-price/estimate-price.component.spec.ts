import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';

import { EstimatePriceComponent } from './estimate-price.component';
import { ReactiveFormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { EstateService } from '../../services/estate.service';

describe('EstimatePriceComponent', () => {
  let component: EstimatePriceComponent;
  let fixture: ComponentFixture<EstimatePriceComponent>;
  let estateService: jasmine.SpyObj<EstateService>;

  beforeEach(async () => {
    const estateServiceSpy = jasmine.createSpyObj('EstateService', ['estimateEstatePrice']);
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    await TestBed.configureTestingModule({
      imports: [EstimatePriceComponent, ReactiveFormsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: EstateService, useValue: estateServiceSpy }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(EstimatePriceComponent);
    component = fixture.componentInstance;
    estateService = TestBed.inject(EstateService) as jasmine.SpyObj<EstateService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form on init', () => {
    expect(component.estimatePriceForm).toBeDefined();
    expect(component.estimatePriceForm.controls['price']).toBeDefined();
  });

  it('should reset the form on init', () => {
    spyOn(component.estimatePriceForm, 'reset');
    component.ngOnInit();
    expect(component.estimatePriceForm.reset).toHaveBeenCalled();
  });

  it('should set isLoading to true and call estimateEstatePrice on form submit', fakeAsync(() => {
    component.estimatePriceForm.setValue({
      price: 100000,
      bedrooms: 3,
      bathrooms: 2,
      landsize: 500,
      street: 'Main St',
      city: 'Sample City',
      state: 'Sample State',
      zipcode: '12345',
      housesize: 1500
    });

    estateService.estimateEstatePrice.and.returnValue(of({ estimatedPrice: 120000 }));

    component.onSubmit();
    tick();

    expect(estateService.estimateEstatePrice).toHaveBeenCalledWith(component.estimatePriceForm.value);
  }));

  it('should set price and isLoading to false after receiving data', () => {
    component.estimatePriceForm.setValue({
      price: 100000,
      bedrooms: 3,
      bathrooms: 2,
      landsize: 500,
      street: 'Main St',
      city: 'Sample City',
      state: 'Sample State',
      zipcode: '12345',
      housesize: 1500
    });

    const mockResponse = { estimatedPrice: 120000 };
    estateService.estimateEstatePrice.and.returnValue(of(mockResponse));

    component.onSubmit();

    expect(component.price).toEqual(mockResponse);
    expect(component.isLoading).toBeFalse();
  });

  it('should not call estimateEstatePrice if form is invalid', () => {
    component.estimatePriceForm.setValue({
      price: '',
      bedrooms: 3,
      bathrooms: 2,
      landsize: 500,
      street: 'Main St',
      city: 'Sample City',
      state: 'Sample State',
      zipcode: '12345',
      housesize: 1500
    });

    component.onSubmit();

    expect(estateService.estimateEstatePrice).not.toHaveBeenCalled();
  });

  it('should set isLoading to false if estimateEstatePrice fails', fakeAsync(() => {
    component.estimatePriceForm.setValue({
      price: 100000,
      bedrooms: 3,
      bathrooms: 2,
      landsize: 500,
      street: 'Main St',
      city: 'Sample City',
      state: 'Sample State',
      zipcode: '12345',
      housesize: 1500
    });

    estateService.estimateEstatePrice.and.returnValue(of({ error: 'Error' }));

    component.onSubmit();
    tick();

    expect(component.isLoading).toBeFalse();
  }));
});
