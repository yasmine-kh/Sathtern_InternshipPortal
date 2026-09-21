import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'register' },
  {
    path: 'register',
    title: 'Student registration',
    loadComponent: () =>
      import('./components/student-registration/student-registration').then(
        (m) => m.StudentRegistration,
      ),
  },
  { path: '**', redirectTo: 'register' },
];
