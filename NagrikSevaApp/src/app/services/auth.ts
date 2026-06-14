import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import * as CryptoJS from 'crypto-js';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5177/api/auth';
  private key = 'NagrikSeva2024!!SecretKey32Chars';

  constructor(private http: HttpClient) {}

  encrypt(text: string): string {
    return CryptoJS.AES.encrypt(text, this.key).toString();
  }

  decrypt(text: string): string {
    return CryptoJS.AES.decrypt(text, this.key)
      .toString(CryptoJS.enc.Utf8);
  }

  register(data: any) {
     const encryptedData = {
    name: this.encrypt(data.name),
    email: this.encrypt(data.email),
    password: this.encrypt(data.password),
    phone: this.encrypt(data.phone),
    role: data.role 
    };
  return this.http.post(
    `${this.apiUrl}/register`,
    encryptedData
  ).pipe(
    map((res: any) => {
      // Decrypt response
      const decrypted = this.decrypt(res.data);
      return JSON.parse(decrypted);
    })
  );
}

  login(data: any) {
    return this.http.post(`${this.apiUrl}/login`, {
      email: this.encrypt(data.email),
      password: this.encrypt(data.password)
    }).pipe(
      map((res: any) => {
        const parsed = JSON.parse(this.decrypt(res.data));
        localStorage.setItem('token', parsed.token);
        localStorage.setItem('name', parsed.name);
        localStorage.setItem('email', parsed.email);
        localStorage.setItem('role', parsed.role);
        localStorage.setItem('id', parsed.id.toString());
        return parsed;
      })
    );
  }

  logout() { localStorage.clear(); }
  isLoggedIn() { return !!localStorage.getItem('token'); }
  getRole() { return localStorage.getItem('role') || ''; }
 
  getName() { return localStorage.getItem('name') || ''; }
  getId() {
  const id = localStorage.getItem('id');
  const parsed = parseInt(id || '0');
  console.log('getId called, returning:', parsed); // debug
  return parsed;
}
getEmail() { return localStorage.getItem('email') || ''; }
 }
