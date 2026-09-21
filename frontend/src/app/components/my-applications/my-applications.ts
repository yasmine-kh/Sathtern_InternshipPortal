import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { switchMap, of } from 'rxjs';

import { Application } from '../../models/application';
import { Student } from '../../models/student';
import { ApplicationService } from '../../services/application.service';
import { StudentService } from '../../services/student.service';

type ViewState =
  | { kind: 'prompt' }
  | { kind: 'loading' }
  | { kind: 'loaded'; student: Student; applications: Application[] }
  | { kind: 'unregistered'; email: string }
  | { kind: 'error'; message: string };

@Component({
  selector: 'app-my-applications',
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './my-applications.html',
  styleUrl: './my-applications.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyApplications {
  private readonly fb = inject(FormBuilder);
  private readonly students = inject(StudentService);
  private readonly applications = inject(ApplicationService);

  // Same email-based identification as the apply flow, since there is no auth.
  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected readonly state = signal<ViewState>({ kind: 'prompt' });

  /** Id of the application awaiting withdrawal confirmation, if any. */
  protected readonly confirmingWithdraw = signal<number | null>(null);
  protected readonly withdrawError = signal('');

  protected lookup(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.load(this.form.getRawValue().email.trim());
  }

  private load(email: string): void {
    this.state.set({ kind: 'loading' });
    this.withdrawError.set('');
    this.confirmingWithdraw.set(null);

    this.students
      .findByEmail(email)
      .pipe(
        switchMap((student) => {
          if (!student) {
            this.state.set({ kind: 'unregistered', email });
            return of(null);
          }

          return this.applications
            .getByStudent(student.id)
            .pipe(switchMap((apps) => of({ student, apps })));
        }),
      )
      .subscribe({
        next: (result) => {
          if (!result) {
            return;
          }

          this.state.set({
            kind: 'loaded',
            student: result.student,
            applications: result.apps,
          });
        },
        error: (error: HttpErrorResponse) =>
          this.state.set({ kind: 'error', message: this.describe(error) }),
      });
  }

  protected askWithdraw(id: number): void {
    this.withdrawError.set('');
    this.confirmingWithdraw.set(id);
  }

  protected cancelWithdraw(): void {
    this.confirmingWithdraw.set(null);
  }

  protected confirmWithdraw(id: number): void {
    const current = this.state();
    if (current.kind !== 'loaded') {
      return;
    }

    const email = current.student.email;

    this.applications.withdraw(id).subscribe({
      // Refetch rather than splicing locally, so the list reflects the server.
      next: () => this.load(email),
      error: (error: HttpErrorResponse) => {
        this.confirmingWithdraw.set(null);
        this.withdrawError.set(
          error.status === 404
            ? 'That application was already withdrawn.'
            : this.describe(error),
        );
      },
    });
  }

  protected reset(): void {
    this.state.set({ kind: 'prompt' });
    this.form.reset();
  }

  protected badgeClass(status: string): string {
    return `badge badge--${status.toLowerCase()}`;
  }

  private describe(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Could not reach the server. Check that the backend is running.';
    }

    return error.error?.detail ?? `Something went wrong (HTTP ${error.status}).`;
  }

  protected hasError(code: string): boolean {
    const field = this.form.controls.email;
    return field.touched && field.hasError(code);
  }
}
