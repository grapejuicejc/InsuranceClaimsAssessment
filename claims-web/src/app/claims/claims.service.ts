import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

// These types match the classes in the API's Contracts folder

export type ClaimStatus = 'Submitted' | 'UnderReview' | 'Settled' | 'Rejected';

export interface ClaimListItem {
  id: number;
  referenceNumber: string;
  policyNumber: string;
  coverType: string;
  incidentDate: string;
  status: ClaimStatus;
  waitingForClaimant: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ClaimHistoryItem {
  status: ClaimStatus;
  changedAt: string;
}

export interface InformationRequestItem {
  id: number;
  message: string;
  requestedAt: string;
  responseText: string | null;
  respondedAt: string | null;
}

export interface ClaimDecisionItem {
  outcome: 'Settled' | 'Rejected';
  reason: string;
  settledAmount: number | null;
  decidedAt: string;
}

export interface ClaimDetails extends ClaimListItem {
  description: string;
  estimatedAmount: number | null;
  history: ClaimHistoryItem[];
  informationRequests: InformationRequestItem[];
  decision: ClaimDecisionItem | null;
}

@Injectable({ providedIn: 'root' })
export class ClaimsService {
  private readonly http = inject(HttpClient);

  getMine(): Observable<ClaimListItem[]> {
    return this.http.get<ClaimListItem[]>('/api/claims');
  }

  getById(id: string): Observable<ClaimDetails> {
    return this.http.get<ClaimDetails>(`/api/claims/${encodeURIComponent(id)}`);
  }
}
