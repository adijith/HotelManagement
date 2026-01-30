import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, tap, throwError } from 'rxjs';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  email: string;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/auth';
  private isAuthenticatedSignal = signal<boolean>(false);
  private currentUserSignal = signal<{ username: string; email: string } | null>(null);

  public isAuthenticated = this.isAuthenticatedSignal.asReadonly();
  public currentUser = this.currentUserSignal.asReadonly();

  constructor(private router: Router, private http: HttpClient) {
    // Check if user was previously logged in
    const token = this.getToken();
    if (token) {
      this.isAuthenticatedSignal.set(true);
      const userData = this.getUserData();
      if (userData) {
        this.currentUserSignal.set(userData);
      }
    }
  }

  login(username: string, password: string): Observable<AuthResponse> {
    const loginRequest: LoginRequest = { username, password };

    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, loginRequest).pipe(
      tap(response => {
        this.handleAuthSuccess(response);
      }),
      catchError(this.handleError)
    );
  }

  register(username: string, email: string, password: string): Observable<AuthResponse> {
    const registerRequest: RegisterRequest = { username, email, password };

    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, registerRequest).pipe(
      tap(response => {
        this.handleAuthSuccess(response);
      }),
      catchError(this.handleError)
    );
  }

  private handleAuthSuccess(response: AuthResponse): void {
    // Store token and user data
    localStorage.setItem('authToken', response.token);
    localStorage.setItem('userData', JSON.stringify({
      username: response.username,
      email: response.email
    }));
    localStorage.setItem('tokenExpiry', response.expiresAt);

    // Update signals
    this.isAuthenticatedSignal.set(true);
    this.currentUserSignal.set({
      username: response.username,
      email: response.email
    });

    // Navigate to dashboard
    this.router.navigate(['/dashboard']);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An error occurred during authentication';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.status === 401) {
        errorMessage = 'Invalid username or password';
      } else if (error.status === 400) {
        errorMessage = error.error?.message || 'Invalid request';
      } else if (error.status === 0) {
        errorMessage = 'Cannot connect to server. Please ensure the backend is running.';
      } else {
        errorMessage = `Server error: ${error.status}`;
      }
    }

    return throwError(() => new Error(errorMessage));
  }

  logout(): void {
    // Clear all auth data
    localStorage.removeItem('authToken');
    localStorage.removeItem('userData');
    localStorage.removeItem('tokenExpiry');

    // Update signals
    this.isAuthenticatedSignal.set(false);
    this.currentUserSignal.set(null);

    // Navigate to login
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getUserData(): { username: string; email: string } | null {
    const userData = localStorage.getItem('userData');
    return userData ? JSON.parse(userData) : null;
  }

  isTokenExpired(): boolean {
    const expiry = localStorage.getItem('tokenExpiry');
    if (!expiry) return true;

    return new Date(expiry) < new Date();
  }
}

