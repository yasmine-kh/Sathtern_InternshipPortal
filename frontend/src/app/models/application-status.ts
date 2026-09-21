/**
 * Mirrors the backend ApplicationStatus enum.
 *
 * The API serialises this as a STRING ("Pending" / "Accepted" / "Rejected"),
 * because the backend registers a JsonStringEnumConverter in Program.cs.
 */
export type ApplicationStatus = 'Pending' | 'Accepted' | 'Rejected';

/** All statuses, in workflow order — handy for dropdowns and filters. */
export const APPLICATION_STATUSES: readonly ApplicationStatus[] = [
  'Pending',
  'Accepted',
  'Rejected',
] as const;
