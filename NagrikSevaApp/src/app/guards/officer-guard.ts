import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';

export const officerGuard: CanActivateFn = () => {
  const router = inject(Router);
  const role = localStorage.getItem('role');
  if (role === 'Officer') return true;
  router.navigate(['/login']);
  return false;
};