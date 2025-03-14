import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';

import { AdminGuard } from './admin.guard';
import { UserService } from '../services/user.service';
import { RouterTestingModule } from '@angular/router/testing';
import { of } from 'rxjs';

describe('AdminGuard', () => {
  let userService: jasmine.SpyObj<UserService>;
  let router: Router;
  const executeGuard: CanActivateFn = (route, state) => 
      TestBed.runInInjectionContext(() => new AdminGuard(router, userService).canActivate(route, state));

  beforeEach(() => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['isAdmin']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        { provide: UserService, useValue: userServiceSpy }
      ]
    });

    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('should allow activation if user is admin', () => {
    userService.isAdmin.and.returnValue(true);
    const routeMock: any = { snapshot: {} } as unknown as ActivatedRouteSnapshot;
    const stateMock: any = { snapshot: {}, url: '/admin' } as unknown as RouterStateSnapshot;
    expect(executeGuard(routeMock, stateMock)).toBeTrue();
  });

  it('should not allow activation and redirect if user is not admin', () => {
    spyOn(router, 'navigate');
    const routeMock: any = { snapshot: {} } as unknown as ActivatedRouteSnapshot;
    const stateMock: any = { snapshot: {}, url: '/admin' } as unknown as RouterStateSnapshot;
    expect(executeGuard(routeMock, stateMock)).toBeFalse();
    expect(router.navigate).toHaveBeenCalledWith(['/not-authorized']);
  });
});
