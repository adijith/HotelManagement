import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ClientManagementComponent } from './components/client-management/client-management.component';
import { CalendarBookingComponent } from './components/calendar-booking/calendar-booking.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'client-management', component: ClientManagementComponent, canActivate: [authGuard] },
  { path: 'calendar-booking', component: CalendarBookingComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '/login' }
];
