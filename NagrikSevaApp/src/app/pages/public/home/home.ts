import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../shared/navbar/navbar';
import { FooterComponent } from '../../../shared/footer/footer';
import { ApplicationService } from '../../../services/application';
import {LucideAngularModule,FileText,BadgeIndianRupee,Car,IdCard,Plane,House,CreditCard,FileBadge} from 'lucide-angular';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink,
    NavbarComponent, FooterComponent,LucideAngularModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class HomeComponent implements OnInit, OnDestroy {
  announcements: any[] = [];
  currentSlide = 0;
  slideInterval: any;
  readonly FileText = FileText;
readonly BadgeIndianRupee = BadgeIndianRupee;
readonly Car = Car;
readonly IdCard = IdCard;
readonly Plane = Plane;
readonly House = House;
readonly CreditCard = CreditCard;
readonly FileBadge = FileBadge;

  slides = [
    {
      image: 'assets/images/gov2.png',
      title: 'Digital India Initiative',
      subtitle: 'Empowering Citizens Through Technology',
      tag: 'Government of India'
    },
    {
      image: 'assets/images/nagrik-logo.png',
      title: 'Nagrik Seva Portal',
      subtitle: 'Your Gateway to Government Services',
      tag: 'Serving 1.4 Billion Citizens'
    },
    {
      image: 'https://images.unsplash.com/photo-1548013146-72479768bada?w=1400&q=80',
      title: 'Incredible India',
      subtitle: 'Proud Heritage, Digital Future',
      tag: '🇮🇳 Jai Hind'
    },
    {
      image: 'assets/images/gov.png',
      title: 'Smart Governance',
      subtitle: 'Fast, Transparent and Efficient Services',
      tag: 'Digital Governance'
    }
  ];

  policies = [
    {
      image: 'assets/images/india.jpg',
      title: 'Digital India',
      desc: 'Transforming India into a digitally empowered society and knowledge economy',
      url: 'https://www.digitalindia.gov.in'

    },
    {
      image: 'assets/images/make-in-india.jpg',
      title: 'Make in India',
      desc: 'Encouraging companies to manufacture their products in India',
      url: 'https://www.makeinindia.com'
    },
    {
      image: 'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=600&q=80',
      title: 'Ayushman Bharat',
      desc: 'Universal health protection scheme for vulnerable families across India',
      url: 'https://pmjay.gov.in'

    },
    {
      image:'assets/images/Pradhan-Mantri-Awas2.png',
      title: 'Pradhan Mantri Awas',
      desc: 'Affordable housing for all citizens of India by 2024',
      url: 'https://pmaymis.gov.in'
    },
    {
      image: 'https://images.unsplash.com/photo-1466611653911-95081537e5b7?w=600&q=80',
      title: 'Solar Power India',
      desc: 'National Solar Mission for clean and renewable energy',
      url: 'https://mnre.gov.in'

    },
    {
      image: 'https://images.unsplash.com/photo-1480714378408-67cf0d13bc1b?w=600&q=80',
      title: 'Smart Cities Mission',
      desc: '100 Smart Cities Mission for sustainable urban development',
      url: 'https://smartcities.gov.in'
    }
  ];

  

services = [
  {
    icon: this.FileBadge,
    title: 'Birth Certificate',
    desc: 'Municipal Applications'
  },
  {
    icon: this.BadgeIndianRupee,
    title: 'Income Certificate',
    desc: 'Revenue Department'
  },
  {
    icon: this.Car,
    title: 'Driving License',
    desc: 'Transport Services'
  },
  {
    icon: this.Plane,
    title: 'Passport',
    desc: 'New Application'
  },
  {
    icon: this.IdCard,
    title: 'Aadhar Update',
    desc: 'Biometric & Info'
  },
  {
    icon: this.House,
    title: 'Property Tax',
    desc: 'State Wise Payment'
  },
  {
    icon: this.CreditCard,
    title: 'PAN Card',
    desc: 'New/Update Services'
  },
  {
    icon: this.FileText,
    title: 'Income Tax',
    desc: 'ITR Filings'
  }
];
  constructor(private appService: ApplicationService) {}

  ngOnInit() {
    this.appService.getAnnouncements()
      .subscribe({
        next: (data: any) => {
          this.announcements = data.slice(0, 3);
        },
        error: () => {}
      });
    this.startSlideshow();
  }

  ngOnDestroy() {
    if (this.slideInterval) clearInterval(this.slideInterval);
  }

  startSlideshow() {
    this.slideInterval = setInterval(() => {
      this.nextSlide();
    }, 4000);
  }

  nextSlide() {
    this.currentSlide =
      (this.currentSlide + 1) % this.slides.length;
  }

  prevSlide() {
    this.currentSlide =
      (this.currentSlide - 1 + this.slides.length) %
      this.slides.length;
  }

  goToSlide(index: number) {
    this.currentSlide = index;
  }
  openPolicy(url: string) {
  window.open(url, '_blank');
}
}