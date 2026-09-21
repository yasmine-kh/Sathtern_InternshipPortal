import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { numberAttribute } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { catchError, of, switchMap, tap } from 'rxjs';

import { Application } from '../../models/application';
import { Internship } from '../../models/internship';
import { ApplicationService } from '../../services/application.service';
import { InternshipService } from '../../services/internship.service';
import { StudentService } from '../../services/student.service';

type LoadState = 'loading' | 'loaded' | 'notfound' | 'error';

type ApplyState =
  | { kind: 'idle' }
  | { kind: 'submitting' }
  | { kind: 'success'; application: Application }
  | { kind: 'error'; message: string; unregistered?: boolean };

@Component({
  selector: 'app-internship-detail',
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './internship-detail.html',
  styleUrl: './internship-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InternshipDetail {
  private readonly internships = inject(InternshipService);
  private readonly students = inject(StudentService);
  private readonly applications = inject(ApplicationService);
  private readonly fb = inject(FormBuilder);

  /** Bound from the :id route parameter via withComponentInputBinding(). */
  readonly id = input.required({ transform: numberAttribute });

  protected readonly state = signal<LoadState>('loading');
  protected readonly loadError = signal('');

  protected readonly internship = toSignal(
    toObservable(this.id).pipe(
      tap(() => {
        this.state.set('loading');
        this.applyState.set({ kind: 'idle' });
      }),
      switchMap((id) =>
        this.internships.getById(id).pipe(
          tap(() => this.state.set('loaded')),
          catchError((error: HttpErrorResponse) => {
            if (error.status === 404) {
              this.state.set('notfound');
            } else {
              this.state.set('error');
              this.loadError.set(
                error.status === 0
                  ? 'Could not reach the server. Check that the backend is running.'
                  : `Could not load this internship (HTTP ${error.status}).`,
              );
            }
            return of(null);
          }),
        ),
      ),
    ),
    { initialValue: null as Internship | null },
  );

  // There is no auth yet, so the applicant identifies themselves by the email
  // they registered with.
  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
  });

  protected readonly applyState = signal<ApplyState>({ kind: 'idle' });

  protected apply(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const email = this.form.getRawValue().email.trim();
    const internshipId = this.id();

    this.applyState.set({ kind: 'submitting' });
    this.form.disable({ emitEvent: false });

    this.students
      .findByEmail(email)
      .pipe(
        switchMap((student) => {
          if (!student) {
            this.applyState.set({
              kind: 'error',
              message: `No student is registered with ${email}.`,
              unregistered: true,
            });
            this.form.enable({ emitEvent: false });
            return of(null);
          }

          return this.applications.apply({ studentId: student.id, internshipId });
        }),
      )
      .subscribe({
        next: (application) => {
          if (!application) {
            return;
          }

          this.applyState.set({ kind: 'success', application });
          this.form.enable({ emitEvent: false });
          this.form.reset();
        },
        error: (error: HttpErrorResponse) => {
          this.applyState.set({ kind: 'error', message: this.describe(error) });
          this.form.enable({ emitEvent: false });
        },
      });
  }

  private describe(error: HttpErrorResponse): string {
    if (error.status === 409) {
      return error.error?.detail ?? 'You have already applied to this internship.';
    }

    if (error.status === 404) {
      return error.error?.detail ?? 'That student or internship no longer exists.';
    }

    if (error.status === 0) {
      return 'Could not reach the server. Check that the backend is running.';
    }

    return error.error?.detail ?? `Could not submit the application (HTTP ${error.status}).`;
  }

  protected hasError(code: string): boolean {
    const field = this.form.controls.email;
    return field.touched && field.hasError(code);
  }

  protected applyAgain(): void {
    this.applyState.set({ kind: 'idle' });
  }
}
