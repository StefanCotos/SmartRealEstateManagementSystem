import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EstateService } from '../../services/estate.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../navbar/navbar.component';
import { CloudinaryService } from '../../services/cloudinary.service';
import { ImageService } from '../../services/image.service';
import { Image } from '../../models/image.model';
import { UserService } from '../../services/user.service';
import { FooterComponent } from '../footer/footer.component';
import { Estate } from '../../models/estate.model';

@Component({
  selector: 'app-estate-update',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './estate-update.component.html',
  styleUrl: './estate-update.component.css',
})
export class EstateUpdateComponent implements OnInit {
  estateForm: FormGroup;
  errorMessage: string = '';
  selectedFilesCount: number = 0;
  files: File[] = [];

  constructor(
    private readonly fb: FormBuilder,
    private readonly estateService: EstateService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly cloudinaryService: CloudinaryService,
    private readonly imageService: ImageService,
    private readonly userService: UserService
  ) {
    this.estateForm = this.fb.group({
      userId: [
        '',
        [
          Validators.required,
          Validators.pattern(
            '^(?:\\{{0,1}(?:[0-9a-fA-F]){8}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){12}\\}{0,1})$'
          ),
        ],
      ],
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
      id: [
        '',
        [
          Validators.required,
          Validators.pattern(
            '^(?:\\{{0,1}(?:[0-9a-fA-F]){8}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){4}-(?:[0-9a-fA-F]){12}\\}{0,1})$'
          ),
        ],
      ],
    });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const estateId = this.route.snapshot.paramMap.get('id');
    if (estateId) {
      this.estateService.getEstateById(estateId).subscribe((data) => {
        this.estateForm.patchValue(data);
      });
    }
  }

  onSubmit(): void {
    if (this.estateForm.valid) {
      const formValue = this.estateForm.value;

      formValue.listingData = new Date().toISOString();

      this.estateService.getEstateById(formValue.id).subscribe((estate) => {
        const productIdBackup = estate.productId;
        const priceIdBackup = estate.priceId;
        this.estateService.updateEstate(formValue).subscribe({
          next: (response) => {
            this.uploadFiles(response);

            const estate = formValue;
            estate.productId = productIdBackup;
            estate.priceId = priceIdBackup;
            estate.unitAmount = formValue.price;
            this.estateService
              .updateEstateProduct(estate)
              .subscribe((newProductData) => {
                estate.priceId = newProductData.priceId;
                estate.productId = newProductData.productId;
                estate.zBANIpierduti = newProductData.unitAmount;
                this.updateEstateProduct(estate);

              });

            this.router.navigate(['estates/detail', formValue.id]);
          },
        });
      });
    }
  }

  uploadFiles(estate: Estate): void {
    this.files.forEach((file) => {
      const fileExtension = file.name.split('.').pop();
      const image: Image = {
        estateId: estate,
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
    });
  }

  updateEstateProduct(estate: Estate): void {
    this.estateService.updateEstate(estate).subscribe(() => {
    });
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
