/**
 * Mirrors the backend ApplicationStatus enum.
 *
 * The API serialises this as a NUMBER (0/1/2), not a string, because the
 * backend has no JsonStringEnumConverter configured. MySQL stores it as a
 * varchar via an EF value conversion, but that is invisible over the wire.
 */
export enum ApplicationStatus {
  Pending = 0,
  Accepted = 1,
  Rejected = 2,
}

/** Display labels, matching the names the backend dashboard returns. */
export const APPLICATION_STATUS_LABELS: Record<ApplicationStatus, string> = {
  [ApplicationStatus.Pending]: 'Pending',
  [ApplicationStatus.Accepted]: 'Accepted',
  [ApplicationStatus.Rejected]: 'Rejected',
};
