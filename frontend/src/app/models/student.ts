/**
 * Matches StudentDto returned by the API.
 * Navigation properties are no longer serialised, so there is no
 * applications array here.
 */
export interface Student {
  id: number;
  fullName: string;
  email: string;
  phone: string | null;
  university: string | null;
  /** ISO 8601 date-time string. */
  createdAt: string;
}

/** Matches StudentSummary, nested inside an application response. */
export interface StudentSummary {
  id: number;
  fullName: string;
  email: string;
}

/** Shape accepted by POST /api/Students and PUT /api/Students/{id}. */
export interface StudentRequest {
  fullName: string;
  email: string;
  phone?: string | null;
  university?: string | null;
}
