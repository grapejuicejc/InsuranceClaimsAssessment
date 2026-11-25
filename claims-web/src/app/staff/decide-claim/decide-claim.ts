import { DecimalPipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { StaffClaimItem, StaffClaimsService } from '../staff-claims.service';

@Component({
  selector: 'app-decide-claim',
  imports: [DecimalPipe, ReactiveFormsModule],
  templateUrl: './decide-claim.html',
  styleUrl: './decide-claim.css',
})
export class DecideClaim {
  private readonly staffClaims = inject(StaffClaimsService);

  readonly claim = input.required<StaffClaimItem>();
  readonly decided = output<void>();
  readonly cancelled = output<void>();

  protected readonly busy = signal(false);
  protected readonly error = signal('');

  protected readonly form = new FormGroup({
    amount: new FormControl<number | null>(null, Validators.min(0.01)),
    reason: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(2000)],
    }),
  });

  protected settle(): void {
    this.error.set('');

    const { amount, reason } = this.form.getRawValue();

    if (amount === null) {
      this.error.set('Enter an amount to settle.');
      return;
    }

    this.send('Settle', () => this.staffClaims.settle(this.claim().id, amount, reason));
  }

  protected reject(): void {
    this.error.set('');

    const { reason } = this.form.getRawValue();

    this.send('Reject', () => this.staffClaims.reject(this.claim().id, reason));
  }

  private send(action: string, request: () => Observable<StaffClaimItem>): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!confirm(`${action} ${this.claim().referenceNumber}? This cannot be undone.`)) {
      return;
    }

    this.busy.set(true);

    request().subscribe({
      next: () => this.decided.emit(),
      error: (response: HttpErrorResponse) => {
        this.busy.set(false);
        this.error.set(response.error?.title ?? 'Could not save the decision.');
      },
    });
  }
}
