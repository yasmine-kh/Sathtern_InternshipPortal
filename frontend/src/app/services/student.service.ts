import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

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
   * Resolves a student by email address.
   *
   * The API exposes no by-email endpoint, so this filters the full list
   * client-side. Fine for a dev-sized dataset; replace with a dedicated
   * endpoint (the repository already has GetByEmailAsync) before this grows.
   */
  findByEmail(email: string): Observable<Student | null> {
    const needle = email.trim().toLowerCase();

    return this.getAll().pipe(
      map((students) => students.find((s) => s.email.toLowerCase() === needle) ?? null),
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
