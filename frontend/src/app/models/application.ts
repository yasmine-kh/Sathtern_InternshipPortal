import { ApplicationStatus } from './application-status';
import { InternshipSummary } from './internship';
import { StudentSummary } from './student';

/**
 * Matches ApplicationDto returned by the API.
 * The student and internship are flattened summaries, null unless the
 * endpoint loaded them.
 */
export interface Application {
  id: number;
  studentId: number;
  internshipId: number;
  status: ApplicationStatus;
  /** ISO 8601 date-time string. */
  appliedDate: string;
  student: StudentSummary | null;
  internship: InternshipSummary | null;
}

/** Body for POST /api/Applications. */
export interface ApplyRequest {
  studentId: number;
  internshipId: number;
}

/** Body for PUT /api/Applications/{id}/status. */
export interface UpdateStatusRequest {
  status: ApplicationStatus;
}
