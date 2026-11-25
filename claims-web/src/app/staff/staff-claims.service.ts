import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ClaimListItem } from '../claims/claims.service';

// Matches StaffClaimItem in the API
export interface StaffClaimItem extends ClaimListItem {
  description: string;
  estimatedAmount: number | null;
  claimantName: string;
  assignedStaffId: number | null;
  assignedStaffName: string | null;
}

export type StaffView = 'incoming' | 'mine' | 'all';

const QUERIES: Record<StaffView, Record<string, string>> = {
  incoming: { status: 'Submitted' },
  mine: { mine: 'true' },
  all: {},
};

@Injectable({ providedIn: 'root' })
export class StaffClaimsService {
  private readonly http = inject(HttpClient);

  get(view: StaffView): Observable<StaffClaimItem[]> {
    return this.http.get<StaffClaimItem[]>('/api/staff/claims', { params: QUERIES[view] });
  }

  pickUp(id: number): Observable<StaffClaimItem> {
    return this.http.post<StaffClaimItem>(`/api/claims/${id}/pickup`, null);
  }

  settle(id: number, amount: number, reason: string): Observable<StaffClaimItem> {
    return this.http.post<StaffClaimItem>(`/api/claims/${id}/settle`, { amount, reason });
  }

  reject(id: number, reason: string): Observable<StaffClaimItem> {
    return this.http.post<StaffClaimItem>(`/api/claims/${id}/reject`, { reason });
  }
}
