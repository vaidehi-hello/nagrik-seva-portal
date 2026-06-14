import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  name = '';
  email = '';
  password = '';
  confirmPassword = '';
  phone = '';
  role = 'Citizen';
  message = '';
  isError = false;
  isLoading = false;
  showPassword = false;
  emailError = '';
  passwordError = '';

  constructor(
    private auth: AuthService,
    private router: Router) {}

  validateEmail() {
    const pattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!this.email) { this.emailError = ''; return; }
    this.emailError = pattern.test(this.email)
      ? '' : 'Invalid email format!';
  }

  validatePassword() {
    if (!this.password) { this.passwordError = ''; return; }
    if (this.password.length < 8)
      this.passwordError = 'Minimum 8 characters';
    else if (!/[A-Z]/.test(this.password))
      this.passwordError = 'Add uppercase letter';
    else if (!/[a-z]/.test(this.password))
      this.passwordError = 'Add lowercase letter';
    else if (!/[0-9]/.test(this.password))
      this.passwordError = 'Add a number';
    else if (!/[!@#$%^&*]/.test(this.password))
      this.passwordError = 'Add special character (!@#$%)';
    else
      this.passwordError = '';
  }

  getStrength(): number {
    let s = 0;
    if (this.password.length >= 8) s++;
    if (/[A-Z]/.test(this.password)) s++;
    if (/[a-z]/.test(this.password)) s++;
    if (/[0-9]/.test(this.password)) s++;
    if (/[!@#$%^&*]/.test(this.password)) s++;
    return s;
  }

  getStrengthLabel() {
    const s = this.getStrength();
    if (s <= 1) return 'Weak';
    if (s <= 3) return 'Medium';
    return 'Strong';
  }

  getStrengthColor() {
    const s = this.getStrength();
    if (s <= 1) return '#dc2626';
    if (s <= 3) return '#FF6B00';
    return '#16a34a';
  }

  register() {
    if (!this.name || !this.email ||
        !this.password || !this.phone) {
      this.message = 'Please fill all fields!';
      this.isError = true;
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.message = 'Passwords do not match!';
      this.isError = true;
      return;
    }

    if (this.emailError || this.passwordError) {
      this.message = 'Please fix the errors above!';
      this.isError = true;
      return;
    }

    this.isLoading = true;
    this.message = '';

    this.auth.register({
      name: this.name,
      email: this.email,
      password: this.password,
      phone: this.phone,
      role: this.role
    }).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        this.message = res.message ||
          'Registered! Please verify your email.';
        this.isError = false;
        setTimeout(() => this.router.navigate(['/login']), 2500);
      },
      error: (err: any) => {
        this.isLoading = false;
        this.message = err.error?.message || 'Registration failed!';
        this.isError = true;
      }
    });
  }
}
