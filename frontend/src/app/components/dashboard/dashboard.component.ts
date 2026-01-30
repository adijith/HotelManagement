import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  constructor(
    private router: Router,
    public authService: AuthService
  ) {}

  get currentUser() {
    return this.authService.currentUser;
  }

  navigateToClientManagement(): void {
    this.router.navigate(['/client-management']);
  }

  navigateToCalendarBooking(): void {
    this.router.navigate(['/calendar-booking']);
  }

  logout(): void {
    this.authService.logout();
  }
}

