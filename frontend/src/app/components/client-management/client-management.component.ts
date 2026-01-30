import { Component, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientService, Client, CreateClientRequest, UpdateClientRequest } from '../../services/client.service';

@Component({
  selector: 'app-client-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './client-management.component.html',
  styleUrl: './client-management.component.css'
})
export class ClientManagementComponent implements OnInit {
  // Modal states
  showAddModal = signal(false);
  showEditModal = signal(false);
  showDeleteModal = signal(false);

  // Form data
  newClient: CreateClientRequest = { fullName: '', email: '', phone: '' };
  editingClient: Client | null = null;
  editClientData: UpdateClientRequest = { fullName: '', email: '', phone: '' };
  deletingClient: Client | null = null;

  // Success message
  successMessage = signal<string | null>(null);

  constructor(
    private router: Router,
    public clientService: ClientService
  ) {}

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.clientService.getClients().subscribe({
      error: (err) => {
        console.error('Error loading clients:', err);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  // Add Client Modal
  openAddModal(): void {
    this.newClient = { fullName: '', email: '', phone: '' };
    this.showAddModal.set(true);
    this.clientService.clearError();
  }

  closeAddModal(): void {
    this.showAddModal.set(false);
    this.newClient = { fullName: '', email: '', phone: '' };
  }

  addClient(): void {
    if (!this.newClient.fullName.trim()) {
      return;
    }

    this.clientService.createClient(this.newClient).subscribe({
      next: () => {
        this.closeAddModal();
        this.showSuccessMessage('Client added successfully!');
      },
      error: (err) => {
        console.error('Error creating client:', err);
      }
    });
  }

  // Edit Client Modal
  openEditModal(client: Client): void {
    this.editingClient = client;
    this.editClientData = {
      fullName: client.fullName,
      email: client.email || '',
      phone: client.phone || ''
    };
    this.showEditModal.set(true);
    this.clientService.clearError();
  }

  closeEditModal(): void {
    this.showEditModal.set(false);
    this.editingClient = null;
    this.editClientData = { fullName: '', email: '', phone: '' };
  }

  updateClient(): void {
    if (!this.editingClient || !this.editClientData.fullName.trim()) {
      return;
    }

    this.clientService.updateClient(this.editingClient.clientId, this.editClientData).subscribe({
      next: () => {
        this.closeEditModal();
        this.showSuccessMessage('Client updated successfully!');
      },
      error: (err) => {
        console.error('Error updating client:', err);
      }
    });
  }

  // Delete Client Modal
  openDeleteModal(client: Client): void {
    this.deletingClient = client;
    this.showDeleteModal.set(true);
    this.clientService.clearError();
  }

  closeDeleteModal(): void {
    this.showDeleteModal.set(false);
    this.deletingClient = null;
  }

  confirmDelete(): void {
    if (!this.deletingClient) {
      return;
    }

    this.clientService.deleteClient(this.deletingClient.clientId).subscribe({
      next: () => {
        this.closeDeleteModal();
        this.showSuccessMessage('Client deleted successfully!');
      },
      error: (err) => {
        console.error('Error deleting client:', err);
      }
    });
  }

  // Success message
  showSuccessMessage(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => {
      this.successMessage.set(null);
    }, 3000);
  }

  // Format date
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}

