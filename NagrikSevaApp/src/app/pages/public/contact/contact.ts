import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../../shared/navbar/navbar';
import { FooterComponent } from '../../../shared/footer/footer';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, FormsModule,
    NavbarComponent, FooterComponent],
  templateUrl: './contact.html',
  styleUrl: './contact.css'
})
export class ContactComponent {
  name = '';
  email = '';
  subject = '';
  message = '';
  submitted = false;

  submit() {
    if (this.name && this.email && this.message) {
      this.submitted = true;
      this.name = '';
      this.email = '';
      this.subject = '';
      this.message = '';
    }
  }
}