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
  {
    path: 'internships/:id',
    title: 'Internship',
    loadComponent: () =>
      import('./components/internship-detail/internship-detail').then(
        (m) => m.InternshipDetail,
      ),
  },
  {
    path: 'my-applications',
    title: 'My applications',
    loadComponent: () =>
      import('./components/my-applications/my-applications').then((m) => m.MyApplications),
  },
  // Linked from the nav but not built yet.
  {
    path: 'admin',
    title: 'Admin',
    component: ComingSoon,
    data: { feature: 'Admin' },
  },
  { path: '**', redirectTo: 'internships' },
];
