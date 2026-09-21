import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Internship, InternshipRequest } from '../models/internship';
import { API_BASE_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class InternshipService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/Internships`;

  /** GET /api/Internships, optionally filtered by a free-text search. */
  getAll(search?: string): Observable<Internship[]> {
    const params = search?.trim()
      ? new HttpParams().set('search', search.trim())
      : undefined;

    return this.http.get<Internship[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Internship> {
    return this.http.get<Internship>(`${this.baseUrl}/${id}`);
  }

  create(internship: InternshipRequest): Observable<Internship> {
    return this.http.post<Internship>(this.baseUrl, internship);
  }

  update(id: number, internship: InternshipRequest): Observable<Internship> {
    return this.http.put<Internship>(`${this.baseUrl}/${id}`, internship);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
