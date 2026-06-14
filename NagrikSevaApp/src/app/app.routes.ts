import { Routes } from '@angular/router';
import { HomeComponent } from './pages/public/home/home';
import { AboutComponent } from './pages/public/about/about';
import { ServicesComponent } from './pages/public/services/services';
import { ContactComponent } from './pages/public/contact/contact';
import { LoginComponent } from './pages/auth/login/login';
import { RegisterComponent } from './pages/auth/register/register';
import { CitizenComponent } from './pages/dashboards/citizen/citizen';
import { OfficerComponent } from './pages/dashboards/officer/officer';
import { AdminComponent } from './pages/dashboards/admin/admin';
import { authGuard } from './guards/auth-guard';
import { citizenGuard } from './guards/citizen-guard';
import { officerGuard } from './guards/officer-guard';
import { adminGuard } from './guards/admin-guard';


export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  { path: 'services', component: ServicesComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'citizen-dashboard',
    component: CitizenComponent,
    canActivate: [authGuard, citizenGuard]
  },
  {
    path: 'officer-dashboard',
    component: OfficerComponent,
   canActivate: [authGuard, officerGuard]
  },
  {
    path: 'admin-dashboard',
    component: AdminComponent,
    canActivate: [authGuard, adminGuard]
  },
  
  { path: '**', redirectTo: '' }
  
];