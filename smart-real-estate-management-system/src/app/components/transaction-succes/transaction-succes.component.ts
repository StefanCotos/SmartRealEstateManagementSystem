import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EstateService } from '../../services/estate.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-transaction-succes',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './transaction-succes.component.html',
  styleUrl: './transaction-succes.component.css',
})
export class TransactionSuccesComponent implements OnInit {
  estate: any;
  username: string | null = '';
  userId: string = '';

  constructor(
    private readonly estateService: EstateService,
    private readonly router: Router,
    private readonly userService: UserService,
  ) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const estateId = localStorage.getItem('estateBuyId');
    localStorage.removeItem('estateBuyId');
    if (estateId) {
      this.estateService.getEstateById(estateId).subscribe((estate) => {
        this.estateService
          .updateEstateProduct(estate)
          .subscribe((newProductData) => {
            estate.isSold = true;
            estate.buyerId = this.userService.getUserId();
            estate.priceId = newProductData.priceId;
            estate.productId = newProductData.productId;
            this.estateService.updateEstate(estate).subscribe(() => {
              this.goToHome();
            });
          });
      });
    }
  }

  goToHome(): void {
    setTimeout(() => {
      this.router.navigate(['']);
    }, 2000);
  }
}
