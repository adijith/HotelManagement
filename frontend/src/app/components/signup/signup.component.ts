import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css'
})
export class SignupComponent {
  username = signal('');
  email = signal('');
  password = signal('');
  confirmPassword = signal('');
  errorMessage = signal('');
  isLoading = signal(false);

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    // Clear previous error
    this.errorMessage.set('');
    
    // Validate inputs
    if (!this.username() || !this.email() || !this.password() || !this.confirmPassword()) {
      this.errorMessage.set('Please fill in all fields');
      return;
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.email())) {
      this.errorMessage.set('Please enter a valid email address');
      return;
    }

    // Validate password length
    if (this.password().length < 6) {
      this.errorMessage.set('Password must be at least 6 characters long');
      return;
    }

    // Validate password match
    if (this.password() !== this.confirmPassword()) {
      this.errorMessage.set('Passwords do not match');
      return;
    }

    // Set loading state
    this.isLoading.set(true);

    // Call the API
    this.authService.register(this.username(), this.email(), this.password()).subscribe({
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

