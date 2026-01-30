import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = signal('');
  password = signal('');
  errorMessage = signal('');
  isLoading = signal(false);

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    // Clear previous error
    this.errorMessage.set('');

    // Validate inputs
    if (!this.username() || !this.password()) {
      this.errorMessage.set('Please enter both username and password');
      return;
    }

    // Set loading state
    this.isLoading.set(true);

    // Call the API
    this.authService.login(this.username(), this.password()).subscribe({
      next: (response) => {
        // Success - AuthService handles navigation
        this.isLoading.set(false);
      },
      error: (error) => {
        // Handle error
        this.isLoading.set(false);
        this.errorMessage.set(error.message);
      }
    });
  }
}

