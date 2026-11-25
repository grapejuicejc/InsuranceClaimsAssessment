import { DatePipe, DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ClaimStatusPipe } from '../claim-status.pipe';
import { ClaimDetails as Claim, ClaimsService } from '../claims.service';

@Component({
  selector: 'app-claim-details',
  imports: [DatePipe, DecimalPipe, RouterLink, ClaimStatusPipe],
  templateUrl: './claim-details.html',
})
export class ClaimDetails {
  // Null while loading
  protected readonly claim = signal<Claim | null>(null);
  protected readonly error = signal('');

  constructor() {
    const id = inject(ActivatedRoute).snapshot.paramMap.get('id')!;

    inject(ClaimsService)
      .getById(id)
      .subscribe({
        next: (claim) => this.claim.set(claim),
        error: (response: HttpErrorResponse) =>
          this.error.set(
            response.status === 404
              ? 'Claim not found.'
              : 'Could not load the claim. Please try again later.',
          ),
      });
  }
}
