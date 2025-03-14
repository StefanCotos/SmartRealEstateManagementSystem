import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EstateService } from '../../services/estate.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { ImageService } from '../../services/image.service';
import { FavoriteService } from '../../services/favorite.service';
import { Estate } from '../../models/estate.model';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs';
import { FooterComponent } from "../footer/footer.component";

@Component({
  selector: 'app-user-favorites-estates',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './user-favorites-estates.component.html',
  styleUrl: './user-favorites-estates.component.css'
})
export class UserFavoritesEstatesComponent implements OnInit {
  favoriteEstates: Estate[] = [];
  paginatedEstates: Estate[] = [];
  currentPage: number = 1;
  itemsPerPage: number = 6;
  imageCache: { [key: string]: string } = {};

  constructor(
    private readonly route: ActivatedRoute,
    private readonly estateService: EstateService,
    private readonly router: Router,
    private readonly userService: UserService,
    private readonly imageService: ImageService,
    private readonly favoriteService: FavoriteService
  ) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.loadFavoriteEstates();
  }

  loadFavoriteEstates(): void {
    const userId = this.userService.getUserId();
    this.favoriteService.getFavoritesByUserId(userId).subscribe(favorites => {
      const estateIds = favorites.map(fav => fav.estateId);
      this.estateService.getEstatesByIds(estateIds).subscribe(estates => {
        this.favoriteEstates = estates;
        this.updatePaginatedEstates();
      });
    });
  }

  updatePaginatedEstates(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.paginatedEstates = this.favoriteEstates.slice(startIndex, endIndex);
    this.paginatedEstates.forEach(estate => {
      if (estate.id) {
        this.imageService.getImagesByEstateId(estate.id).pipe(
          map(images => {
            const imageUrl = images.length > 0
              ? `https://res.cloudinary.com/hlntt5sgz/image/upload/%22${images[0].id}%22`
              : 'https://res.cloudinary.com/hlntt5sgz/image/upload/no_image';
            if (estate.id) {
              this.imageCache[estate.id] = imageUrl;
            }
          }),
          catchError(() => {
            if (estate.id) {
              this.imageCache[estate.id] = 'https://res.cloudinary.com/hlntt5sgz/image/upload/no_image';
            }
            return of(null);
          })
        ).subscribe();
      }
    });
  }

  changePage(increment: boolean): void {
    if (increment) {
      this.currentPage++;
    } else {
      this.currentPage--;
    }
    this.updatePaginatedEstates();
  }

  getPagesArray(): number[] {
    return Array(Math.ceil(this.favoriteEstates.length / this.itemsPerPage)).fill(0).map((x, i) => i + 1);
  }

  navigateToDetailEstate(id: string): void {
    this.router.navigate(['estates/detail', id]);
  }
  
}
