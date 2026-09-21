import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Application, ApplyRequest, UpdateStatusRequest } from '../models/application';
import { ApplicationStatus } from '../models/application-status';
import { API_BASE_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${inject(API_BASE_URL)}/Applications`;

  getAll(status?: ApplicationStatus): Observable<Application[]> {
    const params = status ? new HttpParams().set('status', status) : undefined;
    return this.http.get<Application[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Application> {
    return this.http.get<Application>(`${this.baseUrl}/${id}`);
  }

  /** Applications submitted by one student. */
  getByStudent(studentId: number): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.baseUrl}/student/${studentId}`);
  }

  /** Applications received for one internship. */
  getByInternship(internshipId: number): Observable<Application[]> {
    return this.http.get<Application[]>(`${this.baseUrl}/internship/${internshipId}`);
  }

  /** POST /api/Applications — 409 if this student already applied. */
  apply(request: ApplyRequest): Observable<Application> {
    return this.http.post<Application>(this.baseUrl, request);
  }

  updateStatus(id: number, request: UpdateStatusRequest): Observable<Application> {
    return this.http.put<Application>(`${this.baseUrl}/${id}/status`, request);
  }

  withdraw(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
