import {
  Component, OnInit, NgZone,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import {
  Chart, BarElement, CategoryScale,
  LinearScale, Tooltip, Legend, Title,
  BarController
} from 'chart.js';
import { AuthService } from '../../../services/auth';
import { ApplicationService }
  from '../../../services/application';
import {
  LucideAngularModule,
  LayoutDashboard, FileText, LogOut,
  Menu, ChevronDown, Clock, CheckCircle,
  XCircle, Search, User, Shield,
  ArrowRight, Edit, BarChart2,
  PanelLeftOpen, PanelLeftClose,
  ShieldCheck, ClipboardList, Home,
  Eye, FileCheck2, AlertCircle, X
} from 'lucide-angular';

Chart.register(
  BarElement, CategoryScale, LinearScale,
  Tooltip, Legend, Title, BarController
);

@Component({
  selector: 'app-officer',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    BaseChartDirective,
    LucideAngularModule
  ],
  templateUrl: './officer.html',
  styleUrl: './officer.css'
})
export class OfficerComponent implements OnInit {
  readonly icons = {
    LayoutDashboard, FileText, LogOut,
    Menu, ChevronDown, Clock, CheckCircle,
    XCircle, Search, User, Shield,
    ArrowRight, Edit, BarChart2,
    PanelLeftOpen, PanelLeftClose,
    ShieldCheck, ClipboardList, Home,
    Eye, FileCheck2, AlertCircle, X
  };

  userName = '';
  userInitial = '';
  officerId = 0;
  applications: any[] = [];
  activeTab = 'dashboard';
  message = '';
  isError = false;
  selectedAppId = 0;
  selectedStatus = '';
  remarks = '';
  sidebarCollapsed = false;
  sidebarOpen = false;
  searchQuery = '';

  // ✅ Review panel state
  reviewingApp: any = null;

  barChartData: ChartData<'bar'> = {
    labels: ['Pending', 'UnderReview', 'Approved', 'Rejected'],
    datasets: [{
      label: 'Applications',
      data: [0, 0, 0, 0],
      backgroundColor: [
        '#f97316', '#1e3a5f', '#22c55e', '#ef4444'
      ],
      borderRadius: 8
    }]
  };

  barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    plugins: {
      legend: { display: false }
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: { stepSize: 1 },
        grid: { color: '#f1f5f9' }
      },
      x: { grid: { display: false } }
    }
  };

  constructor(
    private auth: AuthService,
    private appService: ApplicationService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.userName = this.auth.getName();
    this.userInitial = this.userName
      ? this.userName[0].toUpperCase() : 'O';
    this.officerId = this.auth.getId
      ? this.auth.getId()
      : parseInt(localStorage.getItem('id') || '0');

    if (!localStorage.getItem('token')) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadApplications();
    this.loadStats();
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
    this.cdr.markForCheck();
  }

  toggleMobileSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
    this.cdr.markForCheck();
  }

  setTab(tab: string) {
    this.activeTab = tab;
    this.sidebarOpen = false;
    this.message = '';
    this.reviewingApp = null;
    this.cdr.markForCheck();
  }

  loadApplications() {
    this.appService.getAllApplications()
      .subscribe({
        next: (data: any) => {
          this.applications = data;
          this.cdr.markForCheck();
        },
        error: () => {
          this.cdr.markForCheck();
        }
      });
  }

  loadStats() {
    this.appService.getOfficerStats()
      .subscribe({
        next: (data: any) => {
          this.barChartData = {
            labels: [
              'Pending', 'UnderReview',
              'Approved', 'Rejected'
            ],
            datasets: [{
              label: 'Applications',
              data: [
                data['Pending'] || 0,
                data['UnderReview'] || 0,
                data['Approved'] || 0,
                data['Rejected'] || 0
              ],
              backgroundColor: [
                '#f97316', '#1e3a5f',
                '#22c55e', '#ef4444'
              ],
              borderRadius: 8
            }]
          };
          this.cdr.markForCheck();
        },
        error: () => {
          this.cdr.markForCheck();
        }
      });
  }

  getCount(status: string): number {
    return this.applications
      .filter(a => a.status === status).length;
  }

  get filteredApplications() {
    if (!this.searchQuery.trim()) {
      return this.applications;
    }
    const q = this.searchQuery.toLowerCase();
    return this.applications.filter(a =>
      a.appNo?.toLowerCase().includes(q) ||
      a.citizen?.name?.toLowerCase().includes(q) ||
      a.service?.serviceName?.toLowerCase().includes(q)
    );
  }

  getStatusClass(status: string) {
    const map: any = {
      'Approved': 'badge-approved',
      'Rejected': 'badge-rejected',
      'UnderReview': 'badge-review',
      'Pending': 'badge-pending'
    };
    return map[status] || 'badge-pending';
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  goHome() {
    this.router.navigate(['/']);
  }

  // ✅ Review panel
  openReview(app: any) {
    this.appService.getApplicationDetails(app.id)
      .subscribe({
        next: (data: any) => {
          this.reviewingApp = data;
          this.selectedAppId = data.id;
          this.selectedStatus = data.status;
          this.remarks = '';
          this.message = '';
          this.cdr.markForCheck();
        },
        error: () => {
          this.message = 'Failed to load application details!';
          this.isError = true;
          this.cdr.markForCheck();
        }
      });
  }

  closeReview() {
    this.reviewingApp = null;
    this.message = '';
    this.cdr.markForCheck();
  }

  viewDocument(appId: number, docType: 'aadhaar' | 'photo' | 'address') {
    this.appService.getDocument(appId, docType)
      .subscribe({
        next: (blob: any) => {
          const url = window.URL.createObjectURL(blob);
          window.open(url, '_blank');
        },
        error: () => {
          alert('Document not available!');
        }
      });
  }

  updateStatus() {
    if (!this.selectedStatus) {
      this.message = 'Please select a status!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    this.appService.updateStatus(
      this.selectedAppId, {
        status: this.selectedStatus,
        remarks: this.remarks,
        officerId: this.officerId
      }
    ).subscribe({
      next: () => {
        this.message = 'Status updated successfully!';
        this.isError = false;
        this.cdr.markForCheck();
        this.loadApplications();
        this.loadStats();
        setTimeout(() => {
          this.reviewingApp = null;
          this.message = '';
          this.cdr.markForCheck();
        }, 1500);
      },
      error: () => {
        this.message = 'Update failed! Please try again.';
        this.isError = true;
        this.cdr.markForCheck();
      }
    });
  }

  approveWithCertificate() {
    this.appService.approveAndGenerate(this.selectedAppId, {
      remarks: this.remarks,
      officerId: this.officerId,
      status: 'Approved'
    }).subscribe({
      next: () => {
        this.message = 'Approved! Certificate generated successfully.';
        this.isError = false;
        this.cdr.markForCheck();
        this.loadApplications();
        this.loadStats();
        setTimeout(() => {
          this.reviewingApp = null;
          this.message = '';
          this.cdr.markForCheck();
        }, 1800);
      },
      error: () => {
        this.message = 'Approval failed! Please try again.';
        this.isError = true;
        this.cdr.markForCheck();
      }
    });
  }
}