import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PageNotFoundComponent } from './page-not-found.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { SpyLocation } from '@angular/common/testing';


describe('PageNotFoundComponent', () => {
  let component: PageNotFoundComponent;
  let fixture: ComponentFixture<PageNotFoundComponent>;
  let router: Router;
  let location: SpyLocation;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    await TestBed.configureTestingModule({
      imports: [PageNotFoundComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Location, useClass: SpyLocation }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(PageNotFoundComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    location = TestBed.inject(Location) as SpyLocation;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should scroll to top on init', () => {
    spyOn(window, 'scrollTo');
    component.ngOnInit();
    expect(window.scrollTo).toHaveBeenCalledWith(0, 0);
  });

  it('should navigate to home after 2 seconds', (done) => {
    spyOn(router, 'navigate');
    component.ngOnInit();
    setTimeout(() => {
      expect(router.navigate).toHaveBeenCalledWith(['']);
      done();
    }, 2000);
  });
});
