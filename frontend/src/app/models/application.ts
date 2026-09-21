import { ApplicationStatus } from './application-status';
import { Internship } from './internship';
import { Student } from './student';

/**
 * Matches the backend Application entity as serialised by the API.
 * The student/internship navigation properties are null unless the endpoint
 * explicitly loads them.
 */
export interface Application {
  id: number;
  studentId: number;
  student: Student | null;
  internshipId: number;
  internship: Internship | null;
  status: ApplicationStatus;
  /** ISO 8601 date-time string. */
  appliedDate: string;
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
