import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EstateListComponent } from "../estate-list/estate-list.component";
import { NavbarComponent } from '../navbar/navbar.component';
import { UserService } from '../../services/user.service';
import { FooterComponent } from "../footer/footer.component";


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, EstateListComponent, NavbarComponent, FooterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  constructor(private readonly router: Router, public readonly userService: UserService) { }


  navigateToCreateEstate(): void {
    this.router.navigate(['estates/create']);
   }

   isLogged(): boolean {
    if (this.userService.isLoggedIn()) return true;
    return false;
  }
}
