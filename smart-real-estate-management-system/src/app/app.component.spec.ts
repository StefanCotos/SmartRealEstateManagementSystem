import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { provideRouter, Router, RouterModule } from '@angular/router';
import { Location } from '@angular/common';
import { appRoutes } from './app.routes';
import { EstateListComponent } from './components/estate-list/estate-list.component';
import { EstateCreateComponent } from './components/estate-create/estate-create.component';
import { EstateUpdateComponent } from './components/estate-update/estate-update.component';
import { EstateDetailComponent } from './components/estate-detail/estate-detail.component';
import { ApplicationConfig, Component, provideZoneChangeDetection } from '@angular/core';
import { SpyLocation } from '@angular/common/testing';
import { provideClientHydration } from '@angular/platform-browser';
import { appConfig } from './app.config';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthGuard } from './guards/auth.guard';
import { of } from 'rxjs';

@Component({ template: '' })
class DummyComponent { }

describe('AppComponent', () => {
  let router: Router;
  let location: Location;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot(appRoutes),
        EstateListComponent,
        EstateCreateComponent,
        EstateUpdateComponent,
        EstateDetailComponent
      ],
      declarations: [
        DummyComponent
      ],
      providers: [
        { provide: Location, useClass: SpyLocation },
        { provide: AuthGuard, useValue: { canActivate: () => of(true) } },
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    router = TestBed.inject(Router);
    location = TestBed.inject(Location);
    router.initialNavigation();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should have the 'smart-real-estate-management-system' title`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('smart-real-estate-management-system');
  });

  it('should navigate to "estates/create" loads EstateCreateComponent', async () => {
    await router.navigate(['/estates/create']);
    expect(location.path()).toBe('/estates/create');
  });

  it('should navigate to "estates/update/:id" loads EstateUpdateComponent', async () => {
    await router.navigate(['/estates/update/1']);
    expect(location.path()).toBe('/estates/update/1');
  });

  it('should navigate to "estates/detail/:id" loads EstateDetailComponent', async () => {
    await router.navigate(['/estates/detail/1']);
    expect(location.path()).toBe('/estates/detail/1');
  });
});