import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Injectable({
  providedIn: 'root'
})
export class GuestGuard {
  constructor(private readonly router: Router, private readonly userService: UserService) { }

  canActivate: CanActivateFn = (_route, _state) => {
    if (!this.userService.isLoggedIn()) {
      return true;
    } else {
      this.router.navigate(['/profile']);
      return false;
    }
  };
}
