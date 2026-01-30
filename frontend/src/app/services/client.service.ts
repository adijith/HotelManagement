import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface Client {
  clientId: number;
  fullName: string;
  email?: string;
  phone?: string;
  createdAt: string;
}

export interface CreateClientRequest {
  fullName: string;
  email?: string;
  phone?: string;
}

export interface UpdateClientRequest {
  fullName: string;
  email?: string;
  phone?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = 'http://localhost:5000/api/clients';
  
  // Signal to track clients list
  clients = signal<Client[]>([]);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  constructor(private http: HttpClient) {}

  // Get all clients
  getClients(): Observable<Client[]> {
    this.loading.set(true);
    this.error.set(null);
    
    return this.http.get<Client[]>(this.apiUrl).pipe(
      tap({
        next: (clients) => {
          this.clients.set(clients);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Failed to load clients');
          this.loading.set(false);
        }
      })
    );
  }

  // Get client by ID
  getClient(id: number): Observable<Client> {
    return this.http.get<Client>(`${this.apiUrl}/${id}`);
  }

  // Create new client
  createClient(request: CreateClientRequest): Observable<Client> {
    this.loading.set(true);
    this.error.set(null);
    
    return this.http.post<Client>(this.apiUrl, request).pipe(
      tap({
        next: (newClient) => {
          // Add new client to the list
          this.clients.update(clients => [...clients, newClient]);
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Failed to create client');
          this.loading.set(false);
        }
      })
    );
  }

  // Update existing client
  updateClient(id: number, request: UpdateClientRequest): Observable<void> {
    this.loading.set(true);
    this.error.set(null);
    
    return this.http.put<void>(`${this.apiUrl}/${id}`, request).pipe(
      tap({
        next: () => {
          // Update client in the list
          this.clients.update(clients => 
            clients.map(c => c.clientId === id 
              ? { ...c, ...request } 
              : c
            )
          );
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Failed to update client');
          this.loading.set(false);
        }
      })
    );
  }

  // Delete client
  deleteClient(id: number): Observable<void> {
    this.loading.set(true);
    this.error.set(null);
    
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap({
        next: () => {
          // Remove client from the list
          this.clients.update(clients => clients.filter(c => c.clientId !== id));
          this.loading.set(false);
        },
        error: (err) => {
          this.error.set(err.error?.message || 'Failed to delete client');
          this.loading.set(false);
        }
      })
    );
  }

  // Clear error
  clearError(): void {
    this.error.set(null);
  }
}

