import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';

// Temporary page until the claims pages exist
@Component({
  selector: 'app-home',
  templateUrl: './home.html',
})
export class Home {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly session = this.auth.session;

  protected logout(): void {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
