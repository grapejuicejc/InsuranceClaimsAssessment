import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  // Only our own API gets the token, and login does not need one
  if (!request.url.startsWith('/api/') || request.url === '/api/auth/login') {
    return next(request);
  }

  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.session()?.token;

  const apiRequest = token
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(apiRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        auth.logout();
        router.navigateByUrl('/login');
      }

      return throwError(() => error);
    }),
  );
};
