import { Pipe, PipeTransform } from '@angular/core';
import { ClaimStatus } from './claims.service';

const LABELS: Record<ClaimStatus, string> = {
  Submitted: 'Submitted',
  UnderReview: 'Under review',
  Settled: 'Settled',
  Rejected: 'Rejected',
};

@Pipe({ name: 'claimStatus' })
export class ClaimStatusPipe implements PipeTransform {
  transform(status: ClaimStatus): string {
    return LABELS[status];
  }
}
