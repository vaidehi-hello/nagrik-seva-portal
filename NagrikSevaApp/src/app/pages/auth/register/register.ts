import {
  Component, OnInit, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  CheckCircle, Home, Eye, EyeOff
} from 'lucide-angular';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent implements OnInit, OnDestroy {
  readonly icons = { CheckCircle, Home, Eye, EyeOff };

  // ── Form fields
  fullName = '';
  email = '';
  phone = '';
  role = 'Citizen';
  password = '';
  confirmPassword = '';

  // ── UI state
  showPassword = false;
  showConfirmPassword = false;
  message = '';
  isError = false;
  isSubmitting = false;

  // ── Carousel
  slides = [
    'assets/images/india.jpg',
    'assets/images/india2.jpg',
    'assets/images/india3.jpg'
  ];
  currentSlide = 0;
  private slideInterval: any;

  constructor(
    private auth: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.slideInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.slides.length;
      this.cdr.markForCheck();
    }, 4000);
  }

  ngOnDestroy() {
    if (this.slideInterval) clearInterval(this.slideInterval);
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
    this.cdr.markForCheck();
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
    this.cdr.markForCheck();
  }

  goHome() {
    this.router.navigate(['/']);
  }

  onSubmit() {
    this.message = '';
    this.isError = false;

    if (!this.fullName.trim() || !this.email.trim() ||
        !this.phone.trim() || !this.password || !this.confirmPassword) {
      this.message = 'Please fill in all fields!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    if (this.phone.length !== 10) {
      this.message = 'Phone number must be 10 digits!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.message = 'Passwords do not match!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    if (this.password.length < 6) {
      this.message = 'Password must be at least 6 characters!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    this.isSubmitting = true;
    this.cdr.markForCheck();

    const payload = {
      name: this.fullName.trim(),
      email: this.email.trim(),
      phone: this.phone.trim(),
      password: this.password,
      role: this.role
    };

    this.auth.register(payload).subscribe({
      next: (res: any) => {
        this.isSubmitting = false;
        this.isError = false;
        this.message = 'Account created successfully! Redirecting to login...';
        this.cdr.markForCheck();
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1800);
      },
      error: (err: any) => {
        this.isSubmitting = false;
        this.isError = true;
        this.message = err.error?.message || 'Registration failed. Please try again.';
        this.cdr.markForCheck();
      }
    });
  }
}