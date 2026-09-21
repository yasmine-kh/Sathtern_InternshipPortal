import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Stand-in for nav destinations that are linked but not built yet, so those
 * links land somewhere honest instead of falling through to the wildcard
 * redirect. Replace the route with the real component when it exists.
 */
@Component({
  selector: 'app-coming-soon',
  template: `
    <section class="coming-soon">
      <h1>{{ feature() }}</h1>
      <p>This part of the portal hasn't been built yet.</p>
    </section>
  `,
  styles: `
    .coming-soon {
      max-width: 34rem;
      margin: 4rem auto;
      padding: 0 1rem;
      text-align: center;
      color: #5a6472;
      font-family: system-ui, -apple-system, "Segoe UI", sans-serif;
    }

    h1 {
      margin: 0 0 0.5rem;
      font-size: 1.4rem;
      color: #1c2331;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComingSoon {
  readonly feature = input('Coming soon');
}
