import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private apiUrl = 'http://localhost:5177/api/admin';

  constructor(private http: HttpClient) {}

  getDashboardStats() {
    return this.http.get(`${this.apiUrl}/dashboard-stats`);
  }

  getAllUsers() {
    return this.http.get(`${this.apiUrl}/users`);
  }

  getAllApplications() {
    return this.http.get(`${this.apiUrl}/all-applications`);
  }

  deleteUser(id: number) {
    return this.http.delete(`${this.apiUrl}/user/${id}`);
  }

  createAnnouncement(data: any) {
    return this.http.post(`${this.apiUrl}/announcement`, data);
  }

  getAnnouncements() {
    return this.http.get(`${this.apiUrl}/announcements`);
  }
}
