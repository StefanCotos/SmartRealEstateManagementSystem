import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-email-confirmation-valid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './email-confirmation-valid.component.html',
  styleUrl: './email-confirmation-valid.component.css'
})
export class EmailConfirmationValidComponent implements OnInit {

  token: string = '';
  errorMessage: string = '';
  message: string = '';

  constructor(private readonly router: Router, private readonly route: ActivatedRoute, private readonly userService: UserService) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      if (!this.token) {
        this.errorMessage = 'Invalid or missing token';

      }
      else{
        this.userService.confirmEmailValid(this.token).subscribe({
          next: (data) => {
            this.message = data.data;
          },
          error: (err) => {
            this.errorMessage = err.error;
          }
        });

      }
    });
  }


  navigateToLogin(): void {
    this.router.navigate(['login']);
  }
}
