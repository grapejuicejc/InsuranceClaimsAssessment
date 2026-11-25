import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

// Matches LoginResponse in the API
export interface Session {
  token: string;
  expiresAt: string;
  displayName: string;
  role: 'Claimant' | 'ClaimsStaff';
}

const STORAGE_KEY = 'session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentSession = signal<Session | null>(this.readSession());

  readonly session = this.currentSession.asReadonly();

  login(email: string, password: string): Observable<Session> {
    return this.http.post<Session>('/api/auth/login', { email, password }).pipe(
      tap((session) => {
        sessionStorage.setItem(STORAGE_KEY, JSON.stringify(session));
        this.currentSession.set(session);
      }),
    );
  }

  logout(): void {
    sessionStorage.removeItem(STORAGE_KEY);
    this.currentSession.set(null);
  }

  isLoggedIn(): boolean {
    const session = this.currentSession();
    return session !== null && new Date(session.expiresAt) > new Date();
  }

  private readSession(): Session | null {
    const stored = sessionStorage.getItem(STORAGE_KEY);
    return stored ? JSON.parse(stored) : null;
  }
}
