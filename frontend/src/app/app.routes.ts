import { Routes } from '@angular/router';

import { ComingSoon } from './components/coming-soon/coming-soon';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'internships' },
  {
    path: 'register',
    title: 'Student registration',
    loadComponent: () =>
      import('./components/student-registration/student-registration').then(
        (m) => m.StudentRegistration,
      ),
  },
  {
    path: 'internships',
    title: 'Internships',
    loadComponent: () =>
      import('./components/internship-list/internship-list').then((m) => m.InternshipList),
  },
  // Linked from the nav but not built yet.
  {
    path: 'my-applications',
    title: 'My applications',
    component: ComingSoon,
    data: { feature: 'My Applications' },
  },
  {
    path: 'admin',
    title: 'Admin',
    component: ComingSoon,
    data: { feature: 'Admin' },
  },
  { path: '**', redirectTo: 'internships' },
];
