import { TestBed } from '@angular/core/testing';

import { ReportService } from './report.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Report } from '../models/report.model';

describe('ReportService', () => {
  let service: ReportService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReportService]
    });
    service = TestBed.inject(ReportService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create a report', () => {
    const dummyReport: Report = { id: '1', buyerId: '2', sellerId: '3', description: 'This is a test report' };

    service.createReport(dummyReport).subscribe(report => {
      expect(report).toEqual(dummyReport);
    });

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.method).toBe('POST');
    req.flush(dummyReport);
  });

  it('should include Authorization header if token is present', () => {
    const dummyReport: Report = { id: '1', buyerId: '2', sellerId: '3', description: 'This is a test report' };
    const token = 'dummy-token';
    localStorage.setItem('token', token);

    service.createReport(dummyReport).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(dummyReport);
  });

  it('should not include Authorization header if token is not present', () => {
    const dummyReport: Report = { id: '1', buyerId: '2', sellerId: '3', description: 'This is a test report' };
    localStorage.removeItem('token');

    service.createReport(dummyReport).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}`);
    expect(req.request.headers.has('Authorization')).toBeFalsy();
    req.flush(dummyReport);
  });
  
  it('should include Authorization header if token is present when deleting a report', () => {
    const reportId = '1';
    const token = 'dummy-token';
    localStorage.setItem('token', token);

    service.deleteReport(reportId).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}/${reportId}`);
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush(null);
  });

  it('should not include Authorization header if token is not present when deleting a report', () => {
    const reportId = '1';
    localStorage.removeItem('token');

    service.deleteReport(reportId).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}/${reportId}`);
    expect(req.request.headers.has('Authorization')).toBeFalsy();
    req.flush(null);
  });

  it('should get a report by id', () => {
    const reportId = '1';
    const dummyReport: Report = { id: '1', buyerId: '2', sellerId: '3', description: 'This is a test report' };

    service.getReportById(reportId).subscribe(report => {
      expect(report).toEqual(dummyReport);
    });

    const req = httpMock.expectOne(`${service['apiURL']}/${reportId}`);
    expect(req.request.method).toBe('GET');
    req.flush(dummyReport);
  });

  it('should include Authorization header if token is present when getting a report by id', () => {
    const reportId = '1';
    const token = 'dummy-token';
    localStorage.setItem('token', token);

    service.getReportById(reportId).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}/${reportId}`);
    expect(req.request.headers.get('Authorization')).toBe(`Bearer ${token}`);
    req.flush({});
  });

  it('should not include Authorization header if token is not present when getting a report by id', () => {
    const reportId = '1';
    localStorage.removeItem('token');

    service.getReportById(reportId).subscribe();

    const req = httpMock.expectOne(`${service['apiURL']}/${reportId}`);
    expect(req.request.headers.has('Authorization')).toBeFalsy();
    req.flush({});
  });
});
