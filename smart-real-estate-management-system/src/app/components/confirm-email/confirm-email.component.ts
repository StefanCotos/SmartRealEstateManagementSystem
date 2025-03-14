import { Component } from '@angular/core';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent {

  constructor() { }

  ngOnInit(): void {
    window.scrollTo(0, 0);
  }
}
