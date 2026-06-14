import { Component, OnInit } from '@angular/core';
import { RouterLink, Router,RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';


@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;
  userName = '';
  userRole = '';
  searchQuery = '';
  currentLang = 'EN';
  fontSize = 16;

  content: any = {
    EN: {
      portal: 'Nagrik Seva Portal',
      home: 'Home', services: 'Services',
      about: 'About', contact: 'Contact',
      login: 'Login', logout: 'Logout',
      dashboard: 'Dashboard', search: 'Search services...'
    },
    HI: {
      portal: 'नागरिक सेवा पोर्टल',
      home: 'होम', services: 'सेवाएं',
      about: 'हमारे बारे में', contact: 'संपर्क',
      login: 'लॉगिन', logout: 'लॉगआउट',
      dashboard: 'डैशबोर्ड', search: 'सेवाएं खोजें...'
    }
  };

  get lang() { return this.content[this.currentLang]; }

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.isLoggedIn = this.auth.isLoggedIn();
    this.userName = this.auth.getName();
    this.userRole = this.auth.getRole();
  }

  toggleLanguage() {
    this.currentLang = this.currentLang === 'EN' ? 'HI' : 'EN';
  }

  increaseFontSize() {
    if (this.fontSize < 22) {
      this.fontSize++;
      document.documentElement.style.fontSize = this.fontSize + 'px';
    }
  }

  decreaseFontSize() {
    if (this.fontSize > 12) {
      this.fontSize--;
      document.documentElement.style.fontSize = this.fontSize + 'px';
    }
  }

  resetFontSize() {
    this.fontSize = 16;
    document.documentElement.style.fontSize = '16px';
  }

  goToDashboard() {
    const role = this.auth.getRole();
    if (role === 'Admin') this.router.navigate(['/admin-dashboard']);
    else if (role === 'Officer') this.router.navigate(['/officer-dashboard']);
    else this.router.navigate(['/citizen-dashboard']);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  search() {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/services'],
        { queryParams: { q: this.searchQuery } });
    }
  }
}