import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { TransactionSuccesComponent } from './transaction-succes.component';
import { EstateService } from '../../services/estate.service';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { UserService } from '../../services/user.service';

describe('TransactionSucces', () => {
  let component: TransactionSuccesComponent;
  let fixture: ComponentFixture<TransactionSuccesComponent>;
  let estateServiceMock: any;
  let routerMock: any;
  let activatedRouteMock: any;
  let userServiceMock: any;

  beforeEach(async () => {
    estateServiceMock = {
      getEstateById: jasmine.createSpy('getEstateById').and.returnValue(of({})),
      updateEstateProduct: jasmine.createSpy('updateEstateProduct').and.returnValue(of({ priceId: '123', productId: '456', buyerId: null })),
      updateEstate: jasmine.createSpy('updateEstate').and.returnValue(of({}))
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

    userServiceMock = {
      getUserId: jasmine.createSpy('getUserId').and.returnValue('user123'),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('testUser'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
    };

    await TestBed.configureTestingModule({
      imports: [TransactionSuccesComponent],
      providers: [
        { provide: EstateService, useValue: estateServiceMock },
        { provide: Router, useValue: routerMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TransactionSuccesComponent);
    component = fixture.componentInstance;
    component.ngOnInit();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call estateService.getEstateById on ngOnInit and update estate product', () => {
    const estateId = '1';
    localStorage.setItem('estateBuyId', estateId);

    const updatedEstate = {
      id: estateId,
      isSold: false,
      buyerId: null,
      priceId: null,
      productId: null,
    };

    estateServiceMock.getEstateById.and.returnValue(of(updatedEstate));

    component.ngOnInit();

    expect(estateServiceMock.getEstateById).toHaveBeenCalledWith(estateId);
    expect(estateServiceMock.updateEstateProduct).toHaveBeenCalledWith(updatedEstate);
    expect(estateServiceMock.updateEstate).toHaveBeenCalled();
    expect(updatedEstate.isSold).toBe(true);
  });

  it('should call goToHome and navigate to the home page after 2 seconds', (done) => {
    spyOn(component, 'goToHome').and.callThrough();

    component.goToHome();

    setTimeout(() => {
      expect(routerMock.navigate).toHaveBeenCalledWith(['']);
      done();
    }, 2000);
  });
});