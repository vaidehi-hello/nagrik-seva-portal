import {
  Component, OnInit, NgZone,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import {
  Chart, ArcElement, Tooltip,
  Legend, Title, PieController
} from 'chart.js';
import { AuthService } from '../../../services/auth';
import { ApplicationService } from '../../../services/application';
import {
  LucideAngularModule,
  LayoutDashboard, FilePlus, FileText,
  Search, LogOut, Menu, X, Bell,
  ChevronDown, ChevronUp, Plus,
  ArrowRight, FileCheck, Clock,
  CheckCircle, XCircle, PieChart,
  Inbox, Send, Loader, User,
  Phone, Camera, Edit3, Save,
  ChevronRight, Shield, Landmark,
  PanelLeftOpen, PanelLeftClose, ShieldCheck,
  // ✅ NEW icons for apply form
  FolderOpen, FileUp, Download,
  CircleCheck
} from 'lucide-angular';

Chart.register(ArcElement, Tooltip, Legend, Title, PieController);

@Component({
  selector: 'app-citizen',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    FormsModule,
    BaseChartDirective,
    LucideAngularModule
  ],
  templateUrl: './citizen.html',
  styleUrl: './citizen.css'
})
export class CitizenComponent implements OnInit {

  readonly icons = {
    LayoutDashboard, FilePlus, FileText,
    Search, LogOut, Menu, X, Bell,
    ChevronDown, ChevronUp, Plus,
    ArrowRight, FileCheck, Clock,
    CheckCircle, XCircle, PieChart,
    Inbox, Send, Loader, User,
    Phone, Camera, Edit3, Save,
    ChevronRight, Shield, Landmark,
    PanelLeftOpen, PanelLeftClose, ShieldCheck,
    // ✅ NEW
  FolderOpen, FileUp, Download, CircleCheck
  };

  // ── User
  userName = '';
  userInitial = '';
  userId = 0;

  // ── Data
  applications: any[] = [];
  services: any[] = [];

  // ── UI state
  activeTab: string = 'dashboard';
  isSubmitting = false;
  message = '';
  isError = false;
  showAllRecent = false;
  sidebarCollapsed = false;
  sidebarOpen = false;

  // ── Apply
  selectedServiceId: any = 0;
  description = '';

  // ── Track
  trackingNo = '';
  trackedApp: any = null;

  // ── Profile
  showProfileMenu = false;
  showProfileEdit = false;
  editName = '';
  editPhone = '';
  profileEmail = '';
  profileSaved = false;
  profilePhoto = '';           // ← stores base64 image


  //FORM DETAILS:-
  applyStep = 1;
documents = {
  aadhaar: null as File | null,
  photo: null as File | null,
  address: null as File | null
};

applyDetails = {
  fullName: '',
  dateOfBirth: '',
  aadhaarNumber: '',
  fatherName: '',
  address: '',
  mobileNumber: ''
};


  fallbackServices = [
    { id: 1, serviceName: 'Birth Certificate',
      department: { name: 'Revenue Department' } },
    { id: 2, serviceName: 'Income Certificate',
      department: { name: 'Revenue Department' } },
    { id: 3, serviceName: 'Property Tax',
      department: { name: 'Municipal Corporation' } },
    { id: 4, serviceName: 'Driving License',
      department: { name: 'Transport Department' } },
    { id: 5, serviceName: 'Health Card',
      department: { name: 'Health Department' } }
  ];

  pieChartData: ChartData<'pie'> = {
    labels: ['Pending', 'UnderReview', 'Approved', 'Rejected'],
    datasets: [{
      data: [0, 0, 0, 0],
      backgroundColor: ['#f97316', '#1e3a5f', '#22c55e', '#ef4444'],
      borderWidth: 2,
      borderColor: '#fff'
    }]
  };

  pieChartOptions: ChartOptions<'pie'> = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      title: { display: false }
    }
  };

  constructor(
    private auth: AuthService,
    private appService: ApplicationService,
    private router: Router,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.userName    = this.auth.getName();
    this.userInitial = this.userName ? this.userName[0].toUpperCase() : 'U';
    this.editName    = this.userName;
    this.editPhone   = localStorage.getItem('phone') || '';
    this.profileEmail = localStorage.getItem('email') || '';
    this.profilePhoto = localStorage.getItem('profilePhoto') || '';  // ← load saved photo

    const storedId = localStorage.getItem('id');
    this.userId = storedId ? parseInt(storedId) : 0;

    if (!localStorage.getItem('token')) {
      this.router.navigate(['/login']);
      return;
    }
    if (this.userId === 0) {
      this.router.navigate(['/login']);
      return;
    }

    this.loadServices();
    this.loadApplications();
  }

  // ── Sidebar
  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
    this.cdr.markForCheck();
  }

  toggleMobileSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
    this.cdr.markForCheck();
  }

  // ── Profile menu
  toggleProfileMenu() {
    this.showProfileMenu = !this.showProfileMenu;
    this.showProfileEdit = false;
    this.cdr.markForCheck();
  }

  openProfileEdit() {
    this.showProfileEdit = true;
    this.showProfileMenu = false;
    this.activeTab = 'profile';
    this.cdr.markForCheck();
  }
  triggerPhotoUpload() {
  const input = document.getElementById('photoFileInput') as HTMLInputElement;
  if (input) input.click();
}

  // ── Photo upload
  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];
    if (!file.type.startsWith('image/')) {
      this.message = 'Please select an image file.';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      this.ngZone.run(() => {
        this.profilePhoto = reader.result as string;
        localStorage.setItem('profilePhoto', this.profilePhoto);
        this.cdr.markForCheck();
      });
    };
    reader.readAsDataURL(file);
  }

  // ── Save profile text fields
  saveProfile() {
    if (this.editName.trim()) {
      this.userName    = this.editName.trim();
      this.userInitial = this.userName[0].toUpperCase();
      localStorage.setItem('name', this.userName);
    }
    if (this.editPhone.trim()) {
      localStorage.setItem('phone', this.editPhone.trim());
    }
    this.profileSaved = true;
    this.cdr.markForCheck();
    setTimeout(() => {
      this.profileSaved = false;
      this.cdr.markForCheck();
    }, 2500);
  }

  // ── Data loading
  loadServices() {
    this.appService.getServices().subscribe({
      next: (data: any) => {
        this.services = (data && data.length > 0)
          ? data : this.fallbackServices;
        this.cdr.markForCheck();
      },
      error: () => {
        this.services = this.fallbackServices;
        this.cdr.markForCheck();
      }
    });
  }

  loadApplications() {
    const storedId = localStorage.getItem('id');
    const currentUserId = storedId ? parseInt(storedId) : 0;

    if (currentUserId === 0) {
      this.router.navigate(['/login']);
      return;
    }
    this.userId = currentUserId;

    this.appService.getMyApplications(this.userId).subscribe({
      next: (data: any) => {
        this.applications = data;
        this.updateChart();
        this.cdr.markForCheck();
      },
      error: (err: any) => {
        console.error('Apps error:', err);
      }
    });
  }

  updateChart() {
    this.pieChartData = {
      labels: ['Pending', 'UnderReview', 'Approved', 'Rejected'],
      datasets: [{
        data: [
          this.getCount('Pending'),
          this.getCount('UnderReview'),
          this.getCount('Approved'),
          this.getCount('Rejected')
        ],
        backgroundColor: ['#f97316', '#1e3a5f', '#22c55e', '#ef4444'],
        borderWidth: 2,
        borderColor: '#fff'
      }]
    };
  }

  getCount(status: string): number {
    return this.applications.filter(a => a.status === status).length;
  }

  // ── Apply
  
    applyForService() {
  if (!this.documents.aadhaar || !this.documents.photo || !this.documents.address) {
    this.message = 'Please upload a document!';
    this.isError = true;
    this.cdr.markForCheck();
    return;
  }

  this.isSubmitting = true;
  this.message = '';
  this.isError = false;
  this.cdr.markForCheck();

  const formData = new FormData();
  formData.append('citizenId',
    this.userId.toString());
  formData.append('serviceId',
    this.selectedServiceId.toString());
  formData.append('description',
    this.description || '');
  formData.append('fullName',
    this.applyDetails.fullName);
  formData.append('dateOfBirth',
    this.applyDetails.dateOfBirth);
  formData.append('aadhaarNumber',
    this.applyDetails.aadhaarNumber);
  formData.append('fatherName',
    this.applyDetails.fatherName);
  formData.append('address',
    this.applyDetails.address);
  formData.append('mobileNumber',
    this.applyDetails.mobileNumber);
  formData.append('aadhaarDocument', this.documents.aadhaar);
  formData.append('photoDocument', this.documents.photo);
  formData.append('addressDocument', this.documents.address);

  this.appService.applyWithDetails(formData)
    .subscribe({
      next: (res: any) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.isError = false;
          this.message =
            `Submitted! App No: ${res.appNo}`;
          // Reset form
          this.applyStep = 1;
          this.documents = { aadhaar: null, photo: null, address: null };
          this.selectedServiceId = 0;
          this.description = '';
          this.applyDetails = {
            fullName: '', dateOfBirth: '',
            aadhaarNumber: '', fatherName: '',
            address: '', mobileNumber: ''
          };
          this.cdr.markForCheck();
          this.loadApplications();
          setTimeout(() => {
            this.message = '';
            this.activeTab = 'applications';
            this.cdr.markForCheck();
          }, 2500);
        });
      },
      error: (err: any) => {
        this.ngZone.run(() => {
          this.isSubmitting = false;
          this.isError = true;
          this.message =
            err.error?.message || 'Failed!';
          this.cdr.markForCheck();
        });
      }
    });
}

downloadCertificate(appId: number) {
  this.appService.getCertificate(appId)
    .subscribe({
      next: (blob: any) => {
        const url = window.URL
          .createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Certificate_${appId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        alert('Certificate not available yet!');
      }
    });
}

  // ── Track
  trackApp() {
    if (!this.trackingNo.trim()) {
      this.message = 'Please enter an application number!';
      this.isError = true;
      this.cdr.markForCheck();
      return;
    }
    this.appService.trackApplication(this.trackingNo.trim()).subscribe({
      next: (data: any) => {
        this.trackedApp = data;
        this.message = '';
        this.isError = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.trackedApp = null;
        this.message = 'Application not found!';
        this.isError = true;
        this.cdr.markForCheck();
      }
    });
  }

  getStatusClass(status: string) {
    const map: any = {
      'Approved':    'badge-approved',
      'Rejected':    'badge-rejected',
      'UnderReview': 'badge-review',
      'Pending':     'badge-pending'
    };
    return map[status] || 'badge-pending';
  }

  setTab(tab: string) {
    this.activeTab = tab;
    this.sidebarOpen = false;
    this.showProfileMenu = false;
    this.cdr.markForCheck();
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  goHome() {
    this.router.navigate(['/']);
  }

// Next step validation
nextStep() {
  if (this.applyStep === 1) {
    if (!this.selectedServiceId ||
        Number(this.selectedServiceId) === 0) {
      this.message = 'Please select a service!';
      this.isError = true;
      return;
    }
    this.message = '';
    this.isError = false;
    this.applyStep = 2;
  } else if (this.applyStep === 2) {
    if (!this.applyDetails.fullName ||
        !this.applyDetails.aadhaarNumber ||
        !this.applyDetails.address) {
      this.message = 'Please fill all required fields!';
      this.isError = true;
      return;
    }
    this.message = '';
    this.isError = false;
    this.applyStep = 3;
  }
  this.cdr.markForCheck();
}

// ── Document upload (3 separate files)
onFileSelected(event: any, key: 'aadhaar' | 'photo' | 'address') {
  const file = event.target.files[0];
  if (file && file.size > 5 * 1024 * 1024) {
    this.message = 'File size must be under 5MB!';
    this.isError = true;
    this.cdr.markForCheck();
    return;
  }
  this.documents[key] = file || null;
  this.message = '';
  this.isError = false;
  this.cdr.markForCheck();
}

removeDocument(key: 'aadhaar' | 'photo' | 'address') {
  this.documents[key] = null;
  this.cdr.markForCheck();
}



}  

