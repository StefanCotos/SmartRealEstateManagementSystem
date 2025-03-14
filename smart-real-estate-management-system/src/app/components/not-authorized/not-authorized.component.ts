import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-not-authorized',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './not-authorized.component.html',
  styleUrl: './not-authorized.component.css'
})
export class NotAuthorizedComponent implements OnInit {

  constructor(private readonly router: Router) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    setTimeout(() => {
      this.router.navigate(['']);
    }, 2000);
  }
}
