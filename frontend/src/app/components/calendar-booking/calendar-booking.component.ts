import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-calendar-booking',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar-booking.component.html',
  styleUrl: './calendar-booking.component.css'
})
export class CalendarBookingComponent {
  constructor(private router: Router) {}

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }
}

