import { TestBed } from '@angular/core/testing';
import { EstateService } from './estate.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { Estate } from '../models/estate.model';

describe('EstateService', () => {
  let service: EstateService;
  let httpMock: HttpTestingController;

  beforeEach(() => {

    TestBed.configureTestingModule({
      imports: [],
      providers: [
        EstateService,
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(EstateService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve estates from the API via GET', () => {
    const dummyEstates: Estate[] = [
      {
        userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
        name: 'Test Estate1',
        description: 'Test Description1',
        price: 100,
        bedrooms: 3,
        bathrooms: 2,
        landSize: 500,
        street: 'Test Street',
        city: 'Test City',
        state: 'Test State',
        zipCode: '12345',
        houseSize: 150,
        listingData: new Date('2023-01-01T00:00:00.000Z'),
        id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
      },
      {
        userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
        name: 'Test Estate2',
        description: 'Test Description2',
        price: 100,
        bedrooms: 3,
        bathrooms: 2,
        landSize: 500,
        street: 'Test Street',
        city: 'Test City',
        state: 'Test State',
        zipCode: '12345',
        houseSize: 150,
        listingData: new Date('2023-01-01T00:00:00.000Z'),
        id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
      }
    ];

    service.getPaginatedFilterEstates(1, 2).subscribe((estates: Estate[]) => {
      expect(estates).toEqual(dummyEstates);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/filter/paginated?page=1&pageSize=2`);
    expect(request.request.method).toBe('GET');
    request.flush(dummyEstates);
  });

  it('should create a new estate via POST', () => {
    const newEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate3',
      description: 'Test Description3',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    service.createEstate(newEstate).subscribe(estate => {
      expect(estate).toEqual(newEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}`);
    expect(request.request.method).toBe('POST');
    request.flush(newEstate);
  });

  it('should update an existing estate via PUT', () => {
    const updatedEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate4',
      description: 'Test Description4',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    service.updateEstate(updatedEstate).subscribe(estate => {
      expect(estate).toEqual(updatedEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/9eef8dcc-01e5-4c29-8620-860f4aeeeb53`);
    expect(request.request.method).toBe('PUT');
    request.flush(updatedEstate);
  });

  it('should retrieve an estate by ID via GET', () => {
    const dummyEstate: Estate = {
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
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    service.getEstateById('9eef8dcc-01e5-4c29-8620-860f4aeeeb53').subscribe(estate => {
      expect(estate).toEqual(dummyEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/9eef8dcc-01e5-4c29-8620-860f4aeeeb53`);
    expect(request.request.method).toBe('GET');
    request.flush(dummyEstate);
  });

  it('should delete an estate by ID via DELETE', () => {
    service.deleteEstate('9eef8dcc-01e5-4c29-8620-860f4aeeeb53').subscribe(response => {
      expect(response).toBeTruthy();
    });

    const request = httpMock.expectOne(`${service['apiURL']}/9eef8dcc-01e5-4c29-8620-860f4aeeeb53`);
    expect(request.request.method).toBe('DELETE');
    request.flush({});
  });

  it('should buy a product via POST', () => {
    const priceId = 'price_123';
    const response = 'Payment successful';

    service.buyProduct(priceId).subscribe(res => {
      expect(res).toEqual(response);
    });

    const request = httpMock.expectOne(`${service['stripeURL']}/pay`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ defaultPriceId: priceId });
    request.flush(response);
  });

  it('should update an estate product via PUT', () => {
    const estateDetails = { id: 'prod_123', name: 'Updated Product' };
    const response = { success: true };

    service.updateEstateProduct(estateDetails).subscribe(res => {
      expect(res).toEqual(response);
    });

    const request = httpMock.expectOne(`${service['stripeURL']}/updateProduct`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toEqual(estateDetails);
    request.flush(response);
  });

  it('should retrieve estates by IDs via GET', () => {
    const ids = ['id1', 'id2'];
    const dummyEstates: Estate[] = [
      { id: 'id1', name: 'Estate 1', userId: 'user1', description: '', price: 100, bedrooms: 2, bathrooms: 1, landSize: 200, street: '', city: '', state: '', zipCode: '', houseSize: 100, listingData: new Date() },
      { id: 'id2', name: 'Estate 2', userId: 'user2', description: '', price: 200, bedrooms: 3, bathrooms: 2, landSize: 300, street: '', city: '', state: '', zipCode: '', houseSize: 150, listingData: new Date() }
    ];

    service.getEstatesByIds(ids).subscribe(estates => {
      expect(estates).toEqual(dummyEstates);
    });

    const requests = ids.map(id => httpMock.expectOne(`${service['apiURL']}/${id}`));
    requests.forEach((request, index) => {
      expect(request.request.method).toBe('GET');
      request.flush(dummyEstates[index]);
    });
  });

  it('should retrieve estates by user ID via GET', () => {
    const userId = 'user_123';
    const dummyEstates = [{ id: 'estate1', name: 'Estate 1' }];

    service.getEstatesByUserId(userId).subscribe(estates => {
      expect(estates).toEqual(dummyEstates);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/users/${userId}`);
    expect(request.request.method).toBe('GET');
    request.flush(dummyEstates);
  });

  it('should retrieve estates by buyer ID via GET', () => {
    const buyerId = 'buyer_123';
    const dummyEstates = [{ id: 'estate2', name: 'Estate 2' }];

    service.getEstatesByBuyerId(buyerId).subscribe(estates => {
      expect(estates).toEqual(dummyEstates);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/buyers/${buyerId}`);
    expect(request.request.method).toBe('GET');
    request.flush(dummyEstates);
  });

  it('should retrieve all estates via GET', () => {
    const dummyEstates: Estate[] = [
      { id: 'id1', name: 'Estate 1', userId: 'user1', description: '', price: 100, bedrooms: 2, bathrooms: 1, landSize: 200, street: '', city: '', state: '', zipCode: '', houseSize: 100, listingData: new Date() },
      { id: 'id2', name: 'Estate 2', userId: 'user2', description: '', price: 200, bedrooms: 3, bathrooms: 2, landSize: 300, street: '', city: '', state: '', zipCode: '', houseSize: 150, listingData: new Date() }
    ];

    service.getAllEstates().subscribe(estates => {
      expect(estates).toEqual(dummyEstates);
    });

    const request = httpMock.expectOne(`${service['apiURL']}`);
    expect(request.request.method).toBe('GET');
    request.flush(dummyEstates);
  });

  it('should estimate estate price via POST', () => {
    const estate: Estate = {
      id: 'id1', name: 'Estate 1', userId: 'user1', description: '', price: 100, bedrooms: 2, bathrooms: 1, landSize: 200, street: '', city: '', state: '', zipCode: '', houseSize: 100, listingData: new Date()
    };
    const estimatedPrice = { price: 150 };

    service.estimateEstatePrice(estate).subscribe(price => {
      expect(price).toEqual(estimatedPrice);
    });

    const request = httpMock.expectOne(`http://localhost:5045/api/estate-price-prediction/predict`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(estate);
    request.flush(estimatedPrice);
  });

  it('should create a product in Stripe via POST', () => {
    const product = { name: 'Product 1', description: 'Description 1', unitAmount: 1000 };
    const response = { id: 'prod_123', ...product };

    service.createProductStripe(product.name, product.description, product.unitAmount).subscribe(res => {
      expect(res).toEqual(response);
    });

    const request = httpMock.expectOne(`${service['stripeURL']}/createProduct`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(product);
    request.flush(response);
  });

  it('should create a new estate via POST with auth', () => {
    const newEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate5',
      description: 'Test Description5',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    const token = 'test-token';
    localStorage.setItem('token', token);

    service.createEstate(newEstate).subscribe(estate => {
      expect(estate).toEqual(newEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}`);
    expect(request.request.method).toBe('POST');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush(newEstate);
  });

  it('should update an existing estate via PUT with auth', () => {
    const updatedEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate6',
      description: 'Test Description6',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    const token = 'test-token';
    localStorage.setItem('token', token);

    service.updateEstate(updatedEstate).subscribe(estate => {
      expect(estate).toEqual(updatedEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/9eef8dcc-01e5-4c29-8620-860f4aeeeb53`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush(updatedEstate);
  });

  it('should create a new estate via POST with auth', () => {
    const newEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate5',
      description: 'Test Description5',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    const token = 'test-token';
    localStorage.setItem('token', token);

    service.createEstate(newEstate).subscribe(estate => {
      expect(estate).toEqual(newEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}`);
    expect(request.request.method).toBe('POST');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush(newEstate);
  });

  it('should update an existing estate via PUT with auth', () => {
    const updatedEstate: Estate = {
      userId: 'ee06a4ca-79b7-4ce7-8f3b-354424226a09',
      name: 'Test Estate6',
      description: 'Test Description6',
      price: 100,
      bedrooms: 3,
      bathrooms: 2,
      landSize: 500,
      street: 'Test Street',
      city: 'Test City',
      state: 'Test State',
      zipCode: '12345',
      houseSize: 150,
      listingData: new Date('2023-01-01T00:00:00.000Z'),
      id: '9eef8dcc-01e5-4c29-8620-860f4aeeeb53'
    };

    const token = 'test-token';
    localStorage.setItem('token', token);

    service.updateEstate(updatedEstate).subscribe(estate => {
      expect(estate).toEqual(updatedEstate);
    });

    const request = httpMock.expectOne(`${service['apiURL']}/9eef8dcc-01e5-4c29-8620-860f4aeeeb53`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush(updatedEstate);
  });

  it('should delete an estate by ID via DELETE with auth', () => {
    const estateId = '9eef8dcc-01e5-4c29-8620-860f4aeeeb53';
    const token = 'test-token';
    localStorage.setItem('token', token);

    service.deleteEstate(estateId).subscribe(response => {
      expect(response).toBeTruthy();
    });

    const request = httpMock.expectOne(`${service['apiURL']}/${estateId}`);
    expect(request.request.method).toBe('DELETE');
    expect(request.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    request.flush({});
  });

});
