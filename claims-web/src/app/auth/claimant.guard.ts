import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';
import { AuthService } from './auth.service';

// Claimant pages exist only for claimants: the API does the real checks
export const claimantGuard: CanMatchFn = () => inject(AuthService).session()?.role === 'Claimant';
