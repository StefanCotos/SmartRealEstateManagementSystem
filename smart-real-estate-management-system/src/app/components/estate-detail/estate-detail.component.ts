import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EstateService } from '../../services/estate.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { ImageService } from '../../services/image.service';
import { UserService } from '../../services/user.service';
import { FavoriteService } from '../../services/favorite.service';
import { Favorite } from '../../models/favorite.model';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-estate-detail',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './estate-detail.component.html',
  styleUrl: './estate-detail.component.css',
})
export class EstateDetailComponent implements OnInit {
  estate: any;
  username: string | null = '';
  userId: string = '';
  imageCache: { [key: string]: string } = {};
  images: string[] = [];
  currentSlide: number = 0;
  favoriteEstates: string = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly estateService: EstateService,
    private readonly router: Router,
    public readonly userService: UserService,
    private readonly imageService: ImageService,
    private readonly favoriteService: FavoriteService
  ) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const estateId = this.route.snapshot.paramMap.get('id');
    if (estateId) {
      this.estateService.getEstateById(estateId).subscribe((data) => {
        this.estate = data;
        this.userService.getUserById(this.estate.userId).subscribe((data) => {
          this.userId = data.id ?? '';
          this.username = data.userName;
        });
        this.loadImages(estateId);
      });
    }

    if (this.userService.isLoggedIn()) {
      this.favoriteService
        .getFavoritesByUserId(this.userService.getUserId())
        .subscribe((favorites: Favorite[]) => {
          const currentEstateId = this.route.snapshot.paramMap.get('id');
          const isFavorite = favorites.some(
            (favorite) => favorite.estateId === currentEstateId
          );
          if (isFavorite) {
            if (currentEstateId) {
              this.favoriteEstates = currentEstateId;
            }
          }
        });
    }
  }

  loadImages(estateId: string): void {
    this.imageService.getImagesByEstateId(estateId).subscribe((images) => {
      if (images && images.length > 0) {
        this.images = images.map(
          (image) =>
            `https://res.cloudinary.com/hlntt5sgz/image/upload/%22${image.id}%22`
        );
      } else {
        this.images = [
          'https://res.cloudinary.com/hlntt5sgz/image/upload/no_image',
        ];
      }
    });
  }

  nextSlide(): void {
    this.currentSlide = (this.currentSlide + 1) % this.images.length;
  }

  prevSlide(): void {
    this.currentSlide =
      (this.currentSlide - 1 + this.images.length) % this.images.length;
  }

  buyThisProduct(estate: any) {
    localStorage.setItem('estateBuyId', estate.id);
    this.estateService.buyProduct(estate.priceId).subscribe({
      next: (response: any) => {
        window.location.href = response; // Assuming the backend returns the URL as a plain string
      },
      error: (err) => {
        console.error('Error during purchase:', err);
      },
    });
  }

  onDelete(id: string): void {
    this.estateService.deleteEstate(id).subscribe(() => {
      this.router.navigate(['']);
    });
  }

  onEdit(id: string): void {
    this.router.navigate(['estates/update', id]);
  }

  onReviews(id: string): void {
    this.router.navigate(['review-users/get-all', id]);
  }

  onReport(id: string): void {
    this.router.navigate(['reports/create', id]);
  }

  notCurrentUser() {
    if (!this.userService.isLoggedIn() || this.userId == this.userService.getUserId()) {
      return false;
    }
    return true;
  }

  authUser() {
    if (
      this.userService.isLoggedIn() &&
      this.userId == this.userService.getUserId()
    ) {
      return true;
    }
    return false;
  }

  toggleFavorite(estateId: string): void {
    const userId = this.userService.getUserId();
    if (this.favoriteEstates === estateId) {
      this.favoriteService.deleteFavorite(userId, estateId).subscribe();
      this.favoriteEstates = '';
    } else {
      const favorite: Favorite = { userId, estateId };
      this.favoriteService.saveFavorite(favorite).subscribe();
      this.favoriteEstates = estateId;
    }
  }

  getFavoriteIcon(estateId: string): string {
    return this.favoriteEstates === estateId
      ? '../../../assets/star-filled.svg'
      : '../../../assets/star-outline.svg';
  }
}
