import { TestBed } from '@angular/core/testing';

import { CloudinaryService } from './cloudinary.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpTestingController } from '@angular/common/http/testing';

describe('CloudinaryService', () => {
    let service: CloudinaryService;
    let httpMock: HttpTestingController;


  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });
    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(CloudinaryService);
  });

    afterEach(() => {
      httpMock.verify();
    });

    it('should be created', () => {
      expect(service).toBeTruthy();
    });

    it('should upload a file', () => {
      const mockFile = new File([''], 'test-file.jpg');
      const mockName = 'test-file';
      const mockResponse = { success: true };

      service.uploadFile(mockFile, mockName);

      const req = httpMock.expectOne(service['uploadUrl']);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.has('file')).toBeTruthy();
      expect(req.request.body.has('upload_preset')).toBeTruthy();
      expect(req.request.body.has('public_id')).toBeTruthy();
      req.flush(mockResponse);
    });

    it('should handle upload error', () => {
      const mockFile = new File([''], 'test-file.jpg');
      const mockName = 'test-file';
      const mockError = { status: 500, statusText: 'Server Error' };

      service.uploadFile(mockFile, mockName);

      const req = httpMock.expectOne(service['uploadUrl']);
      req.flush(null, mockError);
    });
  });
