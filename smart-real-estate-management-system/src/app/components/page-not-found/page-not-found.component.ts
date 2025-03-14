import { Component, OnInit } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-page-not-found',
  standalone: true,
  imports: [NavbarComponent],
  templateUrl: './page-not-found.component.html',
  styleUrl: './page-not-found.component.css'
})
export class PageNotFoundComponent implements OnInit {

  constructor(private readonly router: Router) { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
    setTimeout(() => {
      this.router.navigate(['']);
    }, 2000);
  }

}
