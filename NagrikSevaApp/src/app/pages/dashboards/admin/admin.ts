import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { AuthService } from '../../../services/auth';
import { AdminService } from '../../../services/admin';
import { ApplicationService } from '../../../services/application';
import { NavbarComponent } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule,
    BaseChartDirective, NavbarComponent],
  templateUrl: './admin.html',
  styleUrl: './admin.css'
})
export class AdminComponent implements OnInit {
  userName = '';
  adminId = 0;
  activeTab = 'dashboard';
  users: any[] = [];
  applications: any[] = [];
  stats: any = {};
  message = '';
  isError = false;

  announcementTitle = '';
  announcementContent = '';

  // Line Chart — Monthly trend (dummy data)
  lineChartData: ChartData<'line'> = {
    labels: ['Jan','Feb','Mar','Apr','May','Jun',
             'Jul','Aug','Sep','Oct','Nov','Dec'],
    datasets: [{
      label: 'Applications',
      data: [12,19,8,15,22,30,18,25,14,20,28,35],
      borderColor: '#1a3a6b',
      backgroundColor: 'rgba(26,58,107,0.1)',
      fill: true,
      tension: 0.4
    }]
  };

  lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: {
      legend: { display: false },
      title: { display: true, text: 'Monthly Applications Trend' }
    },
    scales: { y: { beginAtZero: true } }
  };

  // Doughnut Chart
  doughnutChartData: ChartData<'doughnut'> = {
    labels: ['Pending', 'Under Review', 'Approved', 'Rejected'],
    datasets: [{
      data: [0, 0, 0, 0],
      backgroundColor: [
        '#FF6B00', '#1a3a6b', '#22c55e', '#dc2626'
      ],
      borderWidth: 2
    }]
  };

  doughnutOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      title: { display: true, text: 'Application Status Distribution' }
    }
  };

  // Bar Chart — Users
  barChartData: ChartData<'bar'> = {
    labels: ['Citizens', 'Officers', 'Admins'],
    datasets: [{
      label: 'Users',
      data: [0, 0, 0],
      backgroundColor: ['#1a3a6b', '#FF6B00', '#22c55e'],
      borderRadius: 6
    }]
  };

  barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    plugins: {
      legend: { display: false },
      title: { display: true, text: 'Users by Role' }
    },
    scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
  };

  constructor(
    private auth: AuthService,
    private adminService: AdminService,
    private appService: ApplicationService,
    private router: Router
  ) {}

  ngOnInit() {
    this.userName = this.auth.getName();
    this.adminId = this.auth.getId();
    this.loadStats();
    this.loadUsers();
    this.loadApplications();
  }

  loadStats() {
    this.adminService.getDashboardStats()
      .subscribe((data: any) => {
        this.stats = data.userStats;

        // Update doughnut chart
        this.doughnutChartData = {
          labels: ['Pending', 'UnderReview', 'Approved', 'Rejected'],
          datasets: [{
            data: [
              data.appStats['Pending'] || 0,
              data.appStats['UnderReview'] || 0,
              data.appStats['Approved'] || 0,
              data.appStats['Rejected'] || 0
            ],
            backgroundColor: [
              '#FF6B00', '#1a3a6b', '#22c55e', '#dc2626'
            ],
            borderWidth: 2
          }]
        };

        // Update bar chart
        this.barChartData = {
          labels: ['Citizens', 'Officers', 'Admins'],
          datasets: [{
            label: 'Users',
            data: [
              (data.userStats['TotalUsers'] || 0) -
              (data.userStats['TotalOfficers'] || 0) - 1,
              data.userStats['TotalOfficers'] || 0,
              1
            ],
            backgroundColor: ['#1a3a6b', '#FF6B00', '#22c55e'],
            borderRadius: 6
          }]
        };
      });
  }

  loadUsers() {
    this.adminService.getAllUsers()
      .subscribe((data: any) => { this.users = data; });
  }

  loadApplications() {
    this.adminService.getAllApplications()
      .subscribe((data: any) => { this.applications = data; });
  }

  deleteUser(id: number) {
    if (!confirm('Are you sure you want to delete this user?')) return;
    this.adminService.deleteUser(id).subscribe({
      next: () => {
        this.message = 'User deleted!';
        this.isError = false;
        this.loadUsers();
        this.loadStats();
      },
      error: () => {
        this.message = 'Delete failed!';
        this.isError = true;
      }
    });
  }

  createAnnouncement() {
    if (!this.announcementTitle || !this.announcementContent) {
      this.message = 'Please fill all fields!';
      this.isError = true;
      return;
    }

    this.adminService.createAnnouncement({
      title: this.announcementTitle,
      content: this.announcementContent,
      createdBy: this.adminId,
      isActive: true
    }).subscribe({
      next: () => {
        this.message = 'Announcement created!';
        this.isError = false;
        this.announcementTitle = '';
        this.announcementContent = '';
      },
      error: () => {
        this.message = 'Failed to create announcement!';
        this.isError = true;
      }
    });
  }

  getStatusClass(status: string) {
    switch(status) {
      case 'Approved': return 'status-approved';
      case 'Rejected': return 'status-rejected';
      case 'UnderReview': return 'status-review';
      default: return 'status-pending';
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}