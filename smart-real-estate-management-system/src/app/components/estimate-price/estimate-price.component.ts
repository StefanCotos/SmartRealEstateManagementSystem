import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { EstateService } from '../../services/estate.service';
import { Router } from '@angular/router';
import { UserService } from '../../services/user.service';
import { NavbarComponent } from '../navbar/navbar.component';
import { FooterComponent } from '../footer/footer.component';

@Component({
  selector: 'app-estimate-price',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './estimate-price.component.html',
  styleUrl: './estimate-price.component.css'
})
export class EstimatePriceComponent implements OnInit {

  estimatePriceForm: FormGroup;
  price: any;
  isLoading: boolean = false;
  dots: string = '';

  constructor(private readonly fb: FormBuilder,
    private readonly estateService: EstateService,
    private readonly router: Router,
    private readonly userService: UserService
  ) {
    this.estimatePriceForm = this.fb.group(
      {
        price: ['', [Validators.required, Validators.min(1)]],
        bedrooms: ['', [Validators.required, Validators.min(1)]],
        bathrooms: ['', [Validators.required, Validators.min(1)]],
        landsize: ['', [Validators.required, Validators.min(1)]],
        street: ['', [Validators.required, Validators.maxLength(100)]],
        city: ['', [Validators.required, Validators.maxLength(100)]],
        state: ['', [Validators.required, Validators.maxLength(100)]],
        zipcode: ['', [Validators.required, Validators.maxLength(15)]],
        housesize: ['', [Validators.required, Validators.min(1)]],

      });
  }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.estimatePriceForm.reset();
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
    if (this.estimatePriceForm.valid) {
      this.isLoading = true;
      const formValue = this.estimatePriceForm.value;

      this.estateService.estimateEstatePrice(formValue).subscribe((data) => {
        this.price = data;
        this.isLoading = false;

      });
    }
  }


}
