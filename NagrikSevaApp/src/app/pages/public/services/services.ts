import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../shared/navbar/navbar';
import { FooterComponent } from '../../../shared/footer/footer';
import { ApplicationService } from '../../../services/application';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [CommonModule, RouterLink,
    NavbarComponent, FooterComponent,FormsModule, ],
  templateUrl: './services.html',
  styleUrl: './services.css'
})
export class ServicesComponent implements OnInit {
  services: any[] = [];
  filteredServices: any[] = [];
  departments: any[] = [];
  searchQuery = '';
  selectedDept = 0;

  constructor(
    private appService: ApplicationService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.appService.getServices().subscribe((data: any) => {
      this.services = data;
      this.filteredServices = data;
    });

    this.appService.getAnnouncements().subscribe();

    // Check for search query from navbar
    this.route.queryParams.subscribe(params => {
      if (params['q']) {
        this.searchQuery = params['q'];
        this.filterServices();
      }
    });
  }

  filterServices() {
    this.filteredServices = this.services.filter(s =>
      s.serviceName.toLowerCase()
        .includes(this.searchQuery.toLowerCase()) ||
      s.description.toLowerCase()
        .includes(this.searchQuery.toLowerCase())
    );
  }
}
