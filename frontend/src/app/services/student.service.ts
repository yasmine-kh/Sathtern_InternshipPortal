import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, of, throwError } from 'rxjs';

import { Student, StudentRequest } from '../models/student';
import { API_BASE_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/Students`;

  getAll(): Observable<Student[]> {
    return this.http.get<Student[]>(this.baseUrl);
  }

  getById(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.baseUrl}/${id}`);
  }

  /**
   * Resolves a student by email address via GET /api/Students/by-email.
   *
   * Resolves to null when nobody is registered with that address, so callers
   * can treat "not found" as a normal outcome rather than an error.
   */
  findByEmail(email: string): Observable<Student | null> {
    const params = new HttpParams().set('email', email.trim());

    return this.http.get<Student>(`${this.baseUrl}/by-email`, { params }).pipe(
      catchError((error: HttpErrorResponse) =>
        error.status === 404 ? of(null) : throwError(() => error),
      ),
    );
  }

  /** POST /api/Students — 201 on success, 409 if the email is taken. */
  register(student: StudentRequest): Observable<Student> {
    return this.http.post<Student>(this.baseUrl, student);
  }

  update(id: number, student: StudentRequest): Observable<Student> {
    return this.http.put<Student>(`${this.baseUrl}/${id}`, student);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
