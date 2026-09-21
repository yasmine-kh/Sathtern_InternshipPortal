import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, debounceTime, distinctUntilChanged, of, startWith, switchMap, tap } from 'rxjs';

import { Internship } from '../../models/internship';
import { InternshipService } from '../../services/internship.service';

type LoadState = 'loading' | 'loaded' | 'error';

@Component({
  selector: 'app-internship-list',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './internship-list.html',
  styleUrl: './internship-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InternshipList {
  private readonly internships = inject(InternshipService);

  protected readonly search = new FormControl('', { nonNullable: true });
  protected readonly state = signal<LoadState>('loading');
  protected readonly errorMessage = signal('');

  /**
   * Search is debounced so typing doesn't fire a request per keystroke, and
   * switchMap drops in-flight responses when a newer term arrives.
   */
  protected readonly results = toSignal(
    this.search.valueChanges.pipe(
      startWith(this.search.value),
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => {
        this.state.set('loading');
        this.errorMessage.set('');
      }),
      switchMap((term) =>
        this.internships.getAll(term).pipe(
          tap(() => this.state.set('loaded')),
          catchError((error: HttpErrorResponse) => {
            this.state.set('error');
            this.errorMessage.set(
              error.status === 0
                ? 'Could not reach the server. Check that the backend is running.'
                : `Could not load internships (HTTP ${error.status}).`,
            );
            return of([] as Internship[]);
          }),
        ),
      ),
    ),
    { initialValue: [] as Internship[] },
  );

  protected clearSearch(): void {
    this.search.setValue('');
  }

  /** Trims the description for the card preview. */
  protected preview(description: string | null): string {
    if (!description) {
      return '';
    }

    return description.length > 160 ? `${description.slice(0, 160).trimEnd()}…` : description;
  }
}
