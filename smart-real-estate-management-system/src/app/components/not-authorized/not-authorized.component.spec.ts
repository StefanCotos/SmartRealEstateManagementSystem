import { NotAuthorizedComponent } from './not-authorized.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { Router } from '@angular/router';

describe('NotAuthorizedComponent', () => {
  let component: NotAuthorizedComponent;
  let fixture: ComponentFixture<NotAuthorizedComponent>;
  let router: Router;

  beforeEach(async () => {
    const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
    localStorage.setItem('token', mockToken);

    await TestBed.configureTestingModule({
      imports: [NotAuthorizedComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(NotAuthorizedComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
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

  it('should navigate to home after 2 seconds', fakeAsync(() => {
    spyOn(router, 'navigate');
    component.ngOnInit();
    tick(2000);
    expect(router.navigate).toHaveBeenCalledWith(['']);
  }));

  it('should not navigate before 2 seconds', fakeAsync(() => {
    spyOn(router, 'navigate');
    component.ngOnInit();
    tick(1000);
    expect(router.navigate).not.toHaveBeenCalled();
    tick(2000);
  }));

  it('should call ngOnInit once', () => {
    spyOn(component, 'ngOnInit').and.callThrough();
    component.ngOnInit();
    expect(component.ngOnInit).toHaveBeenCalledTimes(1);
  });
});