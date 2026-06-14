import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private apiUrl = 'http://localhost:5177/api';

  constructor(private http: HttpClient) {}

  getServices() {
    return this.http.get(`${this.apiUrl}/public/services`);
  }

  getAnnouncements() {
    return this.http.get(`${this.apiUrl}/public/announcements`);
  }

  applyForService(data: any) {
    return this.http.post(`${this.apiUrl}/citizen/apply`, data);
  }

  getMyApplications(citizenId: number) {
    return this.http.get(
      `${this.apiUrl}/citizen/applications/${citizenId}`);
  }

  trackApplication(appNo: string) {
    return this.http.get(`${this.apiUrl}/citizen/track/${appNo}`);
  }

  getCitizenStats(citizenId: number) {
    return this.http.get(`${this.apiUrl}/citizen/stats/${citizenId}`);
  }

  getAllApplications() {
    return this.http.get(`${this.apiUrl}/officer/applications`);
  }

  updateStatus(id: number, data: any) {
    return this.http.put(
      `${this.apiUrl}/officer/update-status/${id}`, data);
  }

  getOfficerStats() {
    return this.http.get(`${this.apiUrl}/officer/stats`);
  }

  // ✅ FIXED: now points to apply-with-details
  applyWithDetails(formData: FormData) {
    return this.http.post(
      `${this.apiUrl}/citizen/apply-with-details`,
      formData
    );
  }

  getCertificate(appId: number) {
    return this.http.get(
      `${this.apiUrl}/citizen/certificate/${appId}`,
      { responseType: 'blob' }
    );
  }

  // ✅ NEW: get uploaded document (for officer document viewer)
  getDocument(appId: number, docType: 'aadhaar' | 'photo' | 'address') {
  return this.http.get(
    `${this.apiUrl}/citizen/document/${appId}/${docType}`,
    { responseType: 'blob' }
  );
}

  // ✅ NEW: get full application details (for officer review)
  getApplicationDetails(appId: number) {
    return this.http.get(
      `${this.apiUrl}/officer/application-details/${appId}`
    );
  }

  // ✅ NEW: approve application + generate certificate
  approveAndGenerate(appId: number, data: any) {
    return this.http.post(
      `${this.apiUrl}/officer/approve/${appId}`,
      data
    );
  }
}