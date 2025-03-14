import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserReportComponent } from './user-report.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { UserService } from '../../services/user.service';
import { ReportService } from '../../services/report.service';

describe('UserReportComponent', () => {
  let component: UserReportComponent;
  let fixture: ComponentFixture<UserReportComponent>;
  let activatedRouteMock: any;
  let userServiceMock: any;
  let reportServiceMock: any;
  let routerMock: any;

  beforeEach(async () => {
    activatedRouteMock = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy('get').and.returnValue('1')
        }
      }
    };

    userServiceMock = {
      getUserId: jasmine.createSpy('getUserId').and.returnValue('123'),
      isLoggedIn: jasmine.createSpy('isLoggedIn').and.returnValue(true),
      getUserName: jasmine.createSpy('getUserName').and.returnValue('John Doe'),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false)
    };

    reportServiceMock = {
      createReport: jasmine.createSpy('createReport').and.returnValue(of({}))
    };

    routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, UserReportComponent],
      providers: [
        { provide: ActivatedRoute, useValue: activatedRouteMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: ReportService, useValue: reportServiceMock },
        { provide: Router, useValue: routerMock },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UserReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form on init', () => {
    expect(component.reportForm).toBeDefined();
    expect(component.reportForm.get('description')).toBeDefined();
  });

  it('should reset the form on init', () => {
    spyOn(component.reportForm, 'reset');
    component.ngOnInit();
    expect(component.reportForm.reset).toHaveBeenCalled();
  });

  it('should not submit the form if it is invalid', () => {
    component.reportForm.setValue({ description: '' });
    component.onSubmit();
    expect(reportServiceMock.createReport).not.toHaveBeenCalled();
  });

  it('should submit the form if it is valid', () => {
    component.reportForm.setValue({ description: 'Test description' });
    component.onSubmit();
    expect(reportServiceMock.createReport).toHaveBeenCalledWith({
      description: 'Test description',
      buyerId: '123',
      sellerId: '1'
    });
    expect(routerMock.navigate).toHaveBeenCalledWith(['']);
  });
});