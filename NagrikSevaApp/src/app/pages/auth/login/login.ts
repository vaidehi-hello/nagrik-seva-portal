import {
  Component, OnInit, OnDestroy,
  ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  LucideAngularModule,
  ShieldCheck, Home, Eye, EyeOff
} from 'lucide-angular';
import { AuthService } from '../../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent implements OnInit, OnDestroy {
  readonly icons = { ShieldCheck, Home, Eye, EyeOff };

  email = '';
  password = '';

  showPassword = false;
  message = '';
  isError = false;
  isSubmitting = false;

  // ── Carousel
  slides = [
    'assets/images/Taj-Mahal.jpg',
    'assets/images/India-Gate.jpg',
    'assets/images/Hawa-Mahal.jpg'
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

  goHome() {
    this.router.navigate(['/']);
  }

  onSubmit() {
    this.message = '';
    this.isError = false;

    if (!this.email.trim() || !this.password) {
      this.message = 'Please enter both email and password!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    this.isSubmitting = true;
    this.cdr.markForCheck();

    this.auth.login({ email: this.email.trim(), password: this.password })
      .subscribe({
        next: (res: any) => {
          this.isSubmitting = false;
          this.isError = false;

          localStorage.setItem('token', res.token);
          localStorage.setItem('id', res.id?.toString() || '0');
          localStorage.setItem('name', res.name || '');
          localStorage.setItem('email', res.email || '');
          localStorage.setItem('phone', res.phone || '');

          const role = (res.role || res.roles?.[0] || '').toLowerCase();

          this.cdr.markForCheck();

          if (role === 'officer') {
            this.router.navigate(['/officer-dashboard']);
          } else if (role === 'admin') {
            this.router.navigate(['/admin-dashboard']);
          } else {
            this.router.navigate(['/citizen-dashboard']);
          }
        },
        error: (err: any) => {
          this.isSubmitting = false;
          this.isError = true;
          this.message = err.error?.message || 'Invalid email or password!';
          this.cdr.markForCheck();
        }
      });
  }
}