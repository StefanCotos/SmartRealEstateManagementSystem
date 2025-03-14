import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { EstateService } from '../../services/estate.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { CloudinaryService } from '../../services/cloudinary.service';
import { ImageService } from '../../services/image.service';
import { Image } from '../../models/image.model';
import { HttpClient } from '@angular/common/http';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-estate-create',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './estate-create.component.html',
  styleUrl: './estate-create.component.css',
})
export class EstateCreateComponent implements OnInit {
  estateForm: FormGroup;
  estimatePriceForm: FormGroup;
  price: any;
  isLoading: boolean = false;
  dots: string = '';
  selectedFilesCount: number = 0;
  files: File[] = [];

  constructor(
    private readonly fb: FormBuilder,
    private readonly estateService: EstateService,
    private readonly http: HttpClient,
    private readonly router: Router,
    private readonly userService: UserService,
    private readonly cloudinaryService: CloudinaryService,
    private readonly imageService: ImageService
  ) {
    this.estateForm = this.fb.group({
      id: [''],
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      price: ['', [Validators.required, Validators.min(1)]],
      bedrooms: ['', [Validators.required, Validators.min(1)]],
      bathrooms: ['', [Validators.required, Validators.min(1)]],
      landSize: ['', [Validators.required, Validators.min(1)]],
      street: ['', [Validators.required, Validators.maxLength(100)]],
      city: ['', [Validators.required, Validators.maxLength(50)]],
      state: ['', [Validators.required, Validators.maxLength(50)]],
      zipCode: ['', [Validators.required, Validators.maxLength(15)]],
      houseSize: ['', [Validators.required, Validators.min(1)]],
    });

    this.estimatePriceForm = this.fb.group({
      price: ['', [Validators.required, Validators.min(1)]],
      bedrooms: ['', [Validators.required, Validators.min(1)]],
      bathrooms: ['', [Validators.required, Validators.min(1)]],
      landSize: ['', [Validators.required, Validators.min(1)]],
      street: ['', [Validators.required, Validators.maxLength(100)]],
      city: ['', [Validators.required, Validators.maxLength(50)]],
      state: ['', [Validators.required, Validators.maxLength(50)]],
      zipCode: ['', [Validators.required, Validators.maxLength(15)]],
      houseSize: ['', [Validators.required, Validators.min(1)]],
    });
  }

  copySelectedFieldsToEstimateForm(): void {
    const selectedFields = {
      price: this.estateForm.get('price')?.value,
      bedrooms: this.estateForm.get('bedrooms')?.value,
      bathrooms: this.estateForm.get('bathrooms')?.value,
      landSize: this.estateForm.get('landSize')?.value,
      street: this.estateForm.get('street')?.value,
      city: this.estateForm.get('city')?.value,
      state: this.estateForm.get('state')?.value,
      zipCode: this.estateForm.get('zipCode')?.value,
      houseSize: this.estateForm.get('houseSize')?.value,
    };

    this.estimatePriceForm.patchValue(selectedFields);
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.estateForm.reset();
    this.animateDots();
  }

  animateDots(): void {
    let count = 0;
    setInterval(() => {
      count = (count + 1) % 4;
      this.dots = '.'.repeat(count);
    }, 500);
  }

  onSubmit(): void {
    if (this.estateForm.valid) {
      const formValue = this.estateForm.value;

      // Obține UserId-ul curent
      const userId = this.userService.getUserId();

      if (!userId) {
        console.error('User ID is not available');
        return;
      }

      formValue.userId = userId;

      // Adaugă data curentă
      formValue.listingData = new Date().toISOString();

      // Trimite cererea la API pentru a crea anunțul
      this.estateService.createEstate(formValue).subscribe({
        next: (response) => {
          this.files.forEach((file) => {
            const fileExtension = file.name.split('.').pop();
            if (response) {
              const image: Image = {
                estateId: response,
                extension: fileExtension!,
              };

              this.imageService.saveImage(image).subscribe({
                next: (response) => {
                  const imageId = JSON.stringify(response);
                  this.cloudinaryService.uploadFile(file, imageId);
                },
                error: (error) => {
                  console.error(error);
                },
              });
            }
          });
          formValue.id = response;
          // După ce anunțul este creat, trimite cererea la API pentru Stripe
          this.estateService
            .createProductStripe(
              formValue.name,
              formValue.description,
              formValue.price
            )
            .subscribe({
              next: (stripeResponse) => {
                // Actualizează priceId-ul pentru estate
                formValue.priceId = stripeResponse.priceId;
                formValue.productId = stripeResponse.productId;
                this.estateService.updateEstate(formValue).subscribe({
                  next: () => {
                    this.router.navigate(['']);
                  },
                  error: (error) => {
                    console.error('Error updating estate:', error);
                  },
                });
              },
              error: (error) => {
                console.error('Error creating product:', error);
              },
            });
        },
      });
    }
  }

  estimatePrice(): void {
    if (this.estateForm.valid) {
      this.copySelectedFieldsToEstimateForm();
      this.isLoading = true;
      const formValue = this.estimatePriceForm.value;
      this.estateService.estimateEstatePrice(formValue).subscribe((data) => {
        this.price = data;
        this.isLoading = false;
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFilesCount = input.files.length;
      this.files = Array.from(input.files);
    } else {
      this.selectedFilesCount = 0;
    }
  }
}
