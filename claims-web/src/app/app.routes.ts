import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';
import { claimantGuard } from './auth/claimant.guard';
import { ClaimDetails } from './claims/claim-details/claim-details';
import { MyClaims } from './claims/my-claims/my-claims';
import { ReportClaim } from './claims/report-claim/report-claim';
import { Login } from './login/login';
import { StaffClaims } from './staff/staff-claims/staff-claims';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: '', component: MyClaims, canMatch: [claimantGuard], canActivate: [authGuard] },
  // Before claims/:id, or "new" would be read as an ID
  {
    path: 'claims/new',
    component: ReportClaim,
    canMatch: [claimantGuard],
    canActivate: [authGuard],
  },
  {
    path: 'claims/:id',
    component: ClaimDetails,
    canMatch: [claimantGuard],
    canActivate: [authGuard],
  },
  // Staff, and anyone not logged in (the guard sends them to login)
  { path: '', component: StaffClaims, canActivate: [authGuard] },
  { path: '**', redirectTo: '' },
];
