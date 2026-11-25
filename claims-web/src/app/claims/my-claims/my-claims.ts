import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ClaimStatusPipe } from '../claim-status.pipe';
import { ClaimListItem, ClaimsService } from '../claims.service';

@Component({
  selector: 'app-my-claims',
  imports: [DatePipe, RouterLink, ClaimStatusPipe],
  templateUrl: './my-claims.html',
})
export class MyClaims {
  // Null while loading
  protected readonly claims = signal<ClaimListItem[] | null>(null);
  protected readonly error = signal('');

  constructor() {
    inject(ClaimsService)
      .getMine()
      .subscribe({
        next: (claims) => this.claims.set(claims),
        error: () => this.error.set('Could not load your claims. Please try again later.'),
      });
  }
}
