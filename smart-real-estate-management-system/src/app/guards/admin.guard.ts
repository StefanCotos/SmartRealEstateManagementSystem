import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard {
  constructor(private readonly router: Router, private readonly userService: UserService) { }

  canActivate: CanActivateFn = (_route, _state) => {
    if (this.userService.isAdmin()) {
      return true;
    } else {
      this.router.navigate(['/not-authorized']);
      return false;
    }
  };
}
