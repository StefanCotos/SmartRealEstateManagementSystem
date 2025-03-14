import { Component, OnInit } from '@angular/core';
import { Estate } from '../../models/estate.model';
import { EstateService } from '../../services/estate.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ImageService } from '../../services/image.service';
import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { FavoriteService } from '../../services/favorite.service';
import { Favorite } from '../../models/favorite.model';

@Component({
  selector: 'app-estate-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './estate-list.component.html',
  styleUrls: ['./estate-list.component.css']
})
export class EstateListComponent implements OnInit {
  estates: Estate[] = [];
  currentPage: number = 1;
  totalPages: number = 0;
  pageSize: number = 6;
  pageSizeOptions: number[] = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  filterForm: FormGroup;
  imageCache: { [key: string]: string } = {};
  favoriteEstates: Set<string> = new Set<string>();

  filter = {
    name: '',
    street: '',
    city: '',
    state: '',
    price: 0,
    bedrooms: 0,
    bathrooms: 0,
    landSize: 0,
    houseSize: 0,
  };

  constructor(private readonly estateService: EstateService,
    private readonly router: Router,
    private readonly fb: FormBuilder,
    public imageService: ImageService,
    public userService: UserService,
    public favoriteService: FavoriteService) {
    this.filterForm = this.fb.group({
      name: ['', [Validators.maxLength(100)]],
      street: ['', [Validators.maxLength(100)]],
      city: ['', [Validators.maxLength(50)]],
      state: ['', [Validators.maxLength(50)]],
      price: [0, [Validators.min(0)]],
      bedrooms: [0, [Validators.min(0)]],
      bathrooms: [0, [Validators.min(0)]],
      landSize: [0, [Validators.min(0)]],
      houseSize: [0, [Validators.min(0)]],
    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.loadEstates();
    this.initializeCollapsible();

    if (this.userService.isLoggedIn()) {
      this.favoriteService.getFavoritesByUserId(this.userService.getUserId()).subscribe((favorites: Favorite[]) => {
        favorites.forEach(favorite => {
          this.favoriteEstates.add(favorite.estateId);
        });
      });
    }
  }

  applyFilter(): void {
    this.currentPage = 1; // Reset page to 1 when applying a new filter
    this.loadEstates();
  }

  initializeCollapsible(): void {
    const coll = Array.from(document.getElementsByClassName("filter-collapsible"));
    for (const element of coll) {
      element.addEventListener("click", function (this: HTMLElement) {
        this.classList.toggle("active");
        const content = this.nextElementSibling as HTMLElement;
        if (content.style.display === "block") {
          content.style.display = "none";
        } else {
          content.style.display = "block";
        }
      });
    }
  }

  loadEstates(): void {
    this.estateService.getPaginatedEstates(
      {
        name: this.filter.name,
        street: this.filter.street,
        city: this.filter.city,
        state: this.filter.state,
        price: this.filter.price,
        bedrooms: this.filter.bedrooms,
        bathrooms: this.filter.bathrooms,
        landSize: this.filter.landSize,
        houseSize: this.filter.houseSize
      },
      {
        page: this.currentPage,
        pageSize: this.pageSize
      }
    ).subscribe((data: any) => {
      this.estates = data.data.data/*.filter((estate: Estate) => estate.userId !== this.userService.getUserId())*/;
      this.estates.forEach(estate => {
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

      // this.estateService.getAllEstates().subscribe((allEstates: Estate[]) => {
      //   this.numberOfEstates = allEstates.filter((estate: Estate) => estate.userId !== this.userService.getUserId()).length;
      //   this.totalPages = Math.ceil(this.numberOfEstates / this.pageSize);
      // });

      this.totalPages = Math.ceil(data.data.totalCount / this.pageSize);
    });
  }

  changePage(next: boolean): void {
    if (next && this.currentPage < this.totalPages) {
      this.currentPage++;
    } else if (!next && this.currentPage > 1) {
      this.currentPage--;
    }
    this.loadEstates();
  }

  changePageSize(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const size = selectElement.value;
    if (size) {
      this.pageSize = Number(size);
      this.currentPage = 1;
      this.loadEstates();
    }
  }

  goToPage(page: number): void {
    if (page !== this.currentPage) {
      this.currentPage = page;
      this.loadEstates();
    }
  }

  getPagesArray(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  navigateToHome(): void {
    this.router.navigate(['']);
  }

  navigateToCreateEstate(): void {
    this.router.navigate(['estates/create']);
  }

  navigateToUpdateEstate(id: string): void {
    this.router.navigate(['estates/update', id]);
  }

  navigateToDetailEstate(id: string): void {
    this.router.navigate(['estates/detail', id]);
  }

  toggleFavorite(estateId: string): void {
    const userId = this.userService.getUserId();
    if (this.favoriteEstates.has(estateId)) {
      this.favoriteService.deleteFavorite(userId, estateId).subscribe();
      this.favoriteEstates.delete(estateId);
    } else {
      const favorite: Favorite = { userId, estateId };
      this.favoriteService.saveFavorite(favorite).subscribe();
      this.favoriteEstates.add(estateId);
    }
  }

  getFavoriteIcon(estateId: string): string {
    return this.favoriteEstates.has(estateId) ? '../../../assets/star-filled.svg' : '../../../assets/star-outline.svg';
  }

}
