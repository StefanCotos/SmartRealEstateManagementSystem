import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent {
  currentPage: string = '';
  isMenuExpanded = false;

  constructor(
    private readonly userService: UserService,
    private readonly router: Router
  ) { }

  toggleMenu(): void {
    this.isMenuExpanded = !this.isMenuExpanded;
  }

  navigateToEstateList(): void {
    this.router.navigate(['']);
  }

  navigateToCreateEstate(): void {
    this.router.navigate(['estates/create']);
  }
  navigateToEstimatePrice(): void {
    this.router.navigate(['estates/estimate-price']);
  }

  navigateToLogin(): void {
    this.router.navigate(['login']);
  }

  navigateToRegister(): void {
    this.router.navigate(['register']);
  }
  navigateToContact(): void {
    this.router.navigate(['contact']);
  }

  isLogged(): boolean {
    if (this.userService.isLoggedIn()) return true;
    return false;
  }

  isAdmin(): boolean {
    return this.userService.isAdmin();
  }

  logout(): void {
    this.userService.logout();
    this.router.navigate(['/login']);
  }

  public getUserName(): string | null {
    return this.userService.getUserName();
  }

  navigateToProfile(): void {
    this.router.navigate(['profile']);
  }

  navigateToFavoriteEstates(): void {
    this.router.navigate(['users/favorites']);
  }

  navigateToAdminPage(): void {
    this.router.navigate(['admin']);
  }
}
