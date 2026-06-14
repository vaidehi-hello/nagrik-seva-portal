import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  email = '';
  password = '';
  message = '';
  isError = false;
  isLoading = false;
  showPassword = false;

  constructor(private auth: AuthService, private router: Router) {}

  login() {
    if (!this.email || !this.password) {
      this.message = 'Please fill all fields!';
      this.isError = true;
      return;
    }

    this.isLoading = true;
    this.message = '';

    this.auth.login({ email: this.email, password: this.password })
      .subscribe({
        next: (res: any) => {
          this.isLoading = false;
          this.message = 'Login successful! Redirecting...';
          this.isError = false;

          // Redirect based on role
          setTimeout(() => {
            if (res.role === 'Admin')
              this.router.navigate(['/admin-dashboard']);
            else if (res.role === 'Officer')
              this.router.navigate(['/officer-dashboard']);
            else
              this.router.navigate(['/citizen-dashboard']);
          }, 1000);
        },
        error: (err: any) => {
          this.isLoading = false;
          this.message = err.error?.message || 'Login failed!';
          this.isError = true;
        }
      });
  }
}