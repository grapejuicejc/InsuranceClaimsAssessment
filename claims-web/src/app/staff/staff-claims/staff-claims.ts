import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { Subscription } from 'rxjs';
import { ClaimStatusPipe } from '../../claims/claim-status.pipe';
import { DecideClaim } from '../decide-claim/decide-claim';
import { StaffClaimItem, StaffClaimsService, StaffView } from '../staff-claims.service';

@Component({
  selector: 'app-staff-claims',
  imports: [DatePipe, DecimalPipe, ClaimStatusPipe, DecideClaim],
  templateUrl: './staff-claims.html',
  styleUrl: './staff-claims.css',
})
export class StaffClaims {
  private readonly staffClaims = inject(StaffClaimsService);
  private loading?: Subscription;

  protected readonly views: { id: StaffView; label: string }[] = [
    { id: 'incoming', label: 'Incoming' },
    { id: 'mine', label: 'My workload' },
    { id: 'all', label: 'All claims' },
  ];

  protected readonly view = signal<StaffView>('incoming');
  // Null while loading
  protected readonly claims = signal<StaffClaimItem[] | null>(null);
  protected readonly selected = signal<StaffClaimItem | null>(null);
  protected readonly busy = signal(false);
  protected readonly message = signal('');
  protected readonly error = signal('');

  constructor() {
    this.load();
  }

  protected show(view: StaffView): void {
    this.view.set(view);
    this.message.set('');
    this.error.set('');
    this.load();
  }

  protected pickUp(claim: StaffClaimItem): void {
    this.message.set('');
    this.error.set('');
    this.busy.set(true);

    this.staffClaims.pickUp(claim.id).subscribe({
      next: () => {
        this.busy.set(false);
        this.message.set(`${claim.referenceNumber} is now in your workload.`);
        this.load();
      },
      error: (response: HttpErrorResponse) => {
        this.busy.set(false);
        this.error.set(response.error?.title ?? 'Could not pick up the claim.');
        // Someone else may have taken it, so show the current list
        this.load();
      },
    });
  }

  protected decided(claim: StaffClaimItem): void {
    this.message.set(`${claim.referenceNumber} was decided.`);
    this.error.set('');
    this.load();
  }

  private load(): void {
    // Ignore the answer for a view the user has already left
    this.loading?.unsubscribe();
    this.selected.set(null);
    this.claims.set(null);

    this.loading = this.staffClaims.get(this.view()).subscribe({
      next: (claims) => this.claims.set(claims),
      error: () => {
        this.claims.set([]);
        this.error.set('Could not load the claims. Please try again later.');
      },
    });
  }
}
