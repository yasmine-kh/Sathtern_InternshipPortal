import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';

import { Application } from '../../models/application';
import { ApplicationStatus, APPLICATION_STATUSES } from '../../models/application-status';
import { DashboardSummary } from '../../models/dashboard-summary';
import { Internship } from '../../models/internship';
import { Student } from '../../models/student';
import { AdminService } from '../../services/admin.service';
import { ApplicationService } from '../../services/application.service';
import { InternshipService } from '../../services/internship.service';
import { StudentService } from '../../services/student.service';

type Tab = 'internships' | 'students' | 'applications';
type LoadState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-admin-dashboard',
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AdminDashboard {
  private readonly fb = inject(FormBuilder);
  private readonly admin = inject(AdminService);
  private readonly internshipService = inject(InternshipService);
  private readonly studentService = inject(StudentService);
  private readonly applicationService = inject(ApplicationService);

  protected readonly statuses = APPLICATION_STATUSES;

  protected readonly state = signal<LoadState>('loading');
  protected readonly errorMessage = signal('');
  protected readonly actionError = signal('');

  protected readonly summary = signal<DashboardSummary | null>(null);
  protected readonly internships = signal<Internship[]>([]);
  protected readonly students = signal<Student[]>([]);
  protected readonly applications = signal<Application[]>([]);

  protected readonly tab = signal<Tab>('internships');
  protected readonly statusFilter = signal<ApplicationStatus | 'All'>('All');
  protected readonly confirmingDelete = signal<number | null>(null);

  protected readonly visibleApplications = computed(() => {
    const filter = this.statusFilter();
    const all = this.applications();
    return filter === 'All' ? all : all.filter((a) => a.status === filter);
  });

  protected readonly newInternship = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    company: ['', [Validators.required, Validators.maxLength(150)]],
    location: ['', [Validators.maxLength(150)]],
    duration: ['', [Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(2000)]],
  });

  constructor() {
    this.refresh();
  }

  protected refresh(): void {
    this.state.set('loading');
    this.actionError.set('');
    this.confirmingDelete.set(null);

    forkJoin({
      summary: this.admin.getDashboard(),
      internships: this.internshipService.getAll(),
      students: this.studentService.getAll(),
      applications: this.applicationService.getAll(),
    }).subscribe({
      next: ({ summary, internships, students, applications }) => {
        this.summary.set(summary);
        this.internships.set(internships);
        this.students.set(students);
        this.applications.set(applications);
        this.state.set('loaded');
      },
      error: (error: HttpErrorResponse) => {
        this.state.set('error');
        this.errorMessage.set(
          error.status === 0
            ? 'Could not reach the server. Check that the backend is running.'
            : `Could not load the dashboard (HTTP ${error.status}).`,
        );
      },
    });
  }

  protected selectTab(tab: Tab): void {
    this.tab.set(tab);
    this.actionError.set('');
    this.confirmingDelete.set(null);
  }

  protected createInternship(): void {
    if (this.newInternship.invalid) {
      this.newInternship.markAllAsTouched();
      return;
    }

    const value = this.newInternship.getRawValue();

    this.internshipService
      .create({
        title: value.title.trim(),
        company: value.company.trim(),
        location: value.location.trim() || null,
        duration: value.duration.trim() || null,
        description: value.description.trim() || null,
      })
      .subscribe({
        next: () => {
          this.newInternship.reset();
          this.refresh();
        },
        error: (error: HttpErrorResponse) => this.actionError.set(this.describe(error)),
      });
  }

  protected askDelete(id: number): void {
    this.actionError.set('');
    this.confirmingDelete.set(id);
  }

  protected cancelDelete(): void {
    this.confirmingDelete.set(null);
  }

  protected deleteInternship(id: number): void {
    this.internshipService.delete(id).subscribe({
      next: () => this.refresh(),
      error: (error: HttpErrorResponse) => {
        this.confirmingDelete.set(null);
        this.actionError.set(this.describe(error));
      },
    });
  }

  protected decide(application: Application, status: ApplicationStatus): void {
    this.actionError.set('');

    this.applicationService.updateStatus(application.id, { status }).subscribe({
      next: () => this.refresh(),
      error: (error: HttpErrorResponse) => this.actionError.set(this.describe(error)),
    });
  }

  protected badgeClass(status: string): string {
    return `badge badge--${status.toLowerCase()}`;
  }

  protected statusCount(status: string): number {
    return this.summary()?.applicationsByStatus?.[status] ?? 0;
  }

  protected invalid(control: 'title' | 'company'): boolean {
    const field = this.newInternship.controls[control];
    return field.touched && field.invalid;
  }

  private describe(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Could not reach the server. Check that the backend is running.';
    }

    return error.error?.detail ?? `The action failed (HTTP ${error.status}).`;
  }
}
