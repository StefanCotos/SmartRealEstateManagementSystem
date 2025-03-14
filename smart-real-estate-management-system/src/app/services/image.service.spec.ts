import { TestBed } from '@angular/core/testing';

import { ImageService } from './image.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpTestingController } from '@angular/common/http/testing';
import { Image } from '../models/image.model';
import { Estate } from '../models/estate.model';


describe('ImageService', () => {
  let service: ImageService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [],
      providers: [ImageService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(ImageService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should save an image', () => {
    const dummyEstate: Estate = {
      id: '123', name: 'Estate Name',
      userId: '',
      description: '',
      price: 0,
      bedrooms: 0,
      bathrooms: 0,
      landSize: 0,
      street: '',
      city: '',
      state: '',
      zipCode: '',
      houseSize: 0,
      listingData: new Date()
    };
    const dummyImage: Image = { id: '1', estateId: dummyEstate, extension: 'jpg' };

    service.saveImage(dummyImage).subscribe(image => {
      expect(image).toEqual(dummyImage);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyImage);
  });

  it('should get images by estate id', () => {
    const dummyEstate: Estate = {
      id: '123', name: 'Estate Name',
      userId: '',
      description: '',
      price: 0,
      bedrooms: 0,
      bathrooms: 0,
      landSize: 0,
      street: '',
      city: '',
      state: '',
      zipCode: '',
      houseSize: 0,
      listingData: new Date()
    };
    const dummyImages: Image[] = [
      { id: '1',  estateId: dummyEstate, extension: 'jpg' },
      { id: '2',  estateId: dummyEstate, extension: 'png' }
    ];

    service.getImagesByEstateId('123').subscribe(images => {
      expect(images.length).toBe(2);
      expect(images).toEqual(dummyImages);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/estates/123`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyImages);
  });

  it('should save an image with authorization', () => {
    const dummyEstate: Estate = {
      id: '123', name: 'Estate Name',
      userId: '',
      description: '',
      price: 0,
      bedrooms: 0,
      bathrooms: 0,
      landSize: 0,
      street: '',
      city: '',
      state: '',
      zipCode: '',
      houseSize: 0,
      listingData: new Date()
    };
    const dummyImage: Image = { id: '1', estateId: dummyEstate, extension: 'jpg' };
    const token = 'dummy-token';
    localStorage.setItem('token', token);

    service.saveImage(dummyImage).subscribe(image => {
      expect(image).toEqual(dummyImage);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(dummyImage);

    localStorage.removeItem('token');
  });
});