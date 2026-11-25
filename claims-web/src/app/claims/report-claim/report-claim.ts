import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ClaimsService, PolicyItem } from '../claims.service';

@Component({
  selector: 'app-report-claim',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './report-claim.html',
  styleUrl: './report-claim.css',
})
export class ReportClaim {
  private readonly claimsService = inject(ClaimsService);
  private readonly router = inject(Router);

  // Null while loading
  protected readonly policies = signal<PolicyItem[] | null>(null);
  protected readonly busy = signal(false);
  protected readonly loadError = signal('');
  protected readonly error = signal('');

  protected readonly form = new FormGroup({
    policyId: new FormControl<number | null>(null, Validators.required),
    incidentDate: new FormControl('', { nonNullable: true, validators: Validators.required }),
    description: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(2000)],
    }),
    estimatedAmount: new FormControl<number | null>(null, Validators.min(0.01)),
  });

  constructor() {
    this.claimsService.getPolicies().subscribe({
      next: (policies) => this.policies.set(policies),
      error: () => this.loadError.set('Could not load your policies. Please try again later.'),
    });
  }

  protected submit(): void {
    this.error.set('');

    const { policyId, incidentDate, description, estimatedAmount } = this.form.getRawValue();

    if (this.form.invalid || policyId === null) {
      this.form.markAllAsTouched();
      return;
    }

    this.busy.set(true);

    this.claimsService.create({ policyId, incidentDate, description, estimatedAmount }).subscribe({
      next: (claim) => this.router.navigate(['/claims', claim.id]),
      error: (response: HttpErrorResponse) => {
        this.busy.set(false);
        this.error.set(response.error?.title ?? 'Could not report the claim.');
      },
    });
  }
}
