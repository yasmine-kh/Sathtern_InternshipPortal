import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { Student } from '../../models/student';
import { StudentService } from '../../services/student.service';

type SubmitState =
  | { kind: 'idle' }
  | { kind: 'saving' }
  | { kind: 'success'; student: Student }
  | { kind: 'error'; message: string };

@Component({
  selector: 'app-student-registration',
  imports: [ReactiveFormsModule],
  templateUrl: './student-registration.html',
  styleUrl: './student-registration.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StudentRegistration {
  private readonly fb = inject(FormBuilder);
  private readonly students = inject(StudentService);

  /** Mirrors the backend's validation: full name and a well-formed email. */
  protected readonly form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(150)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
    phone: ['', [Validators.maxLength(30)]],
    university: ['', [Validators.maxLength(200)]],
  });

  protected readonly state = signal<SubmitState>({ kind: 'idle' });

  protected submit(): void {
    if (this.form.invalid) {
      // Surface messages for fields the user never touched.
      this.form.markAllAsTouched();
      return;
    }

    const { fullName, email, phone, university } = this.form.getRawValue();

    this.state.set({ kind: 'saving' });
    this.form.disable({ emitEvent: false });

    this.students
      .register({
        fullName: fullName.trim(),
        email: email.trim(),
        // Send null rather than empty strings for the optional fields.
        phone: phone.trim() || null,
        university: university.trim() || null,
      })
      .subscribe({
        next: (student) => {
          this.state.set({ kind: 'success', student });
          this.form.enable({ emitEvent: false });
          this.form.reset();
        },
        error: (error: HttpErrorResponse) => {
          this.state.set({ kind: 'error', message: this.describe(error) });
          this.form.enable({ emitEvent: false });
        },
      });
  }

  protected reset(): void {
    this.state.set({ kind: 'idle' });
    this.form.reset();
  }

  /** Turns an HTTP failure into something a person can act on. */
  private describe(error: HttpErrorResponse): string {
    if (error.status === 409) {
      return (
        error.error?.detail ??
        'That email address is already registered. Try signing in instead.'
      );
    }

    if (error.status === 0) {
      return 'Could not reach the server. Check that the backend is running.';
    }

    if (error.status === 400 || error.status === 500) {
      return error.error?.detail ?? 'The details provided could not be saved.';
    }

    return `Registration failed (HTTP ${error.status}).`;
  }

  protected hasError(control: 'fullName' | 'email' | 'phone' | 'university', code: string): boolean {
    const field = this.form.controls[control];
    return field.touched && field.hasError(code);
  }
}
