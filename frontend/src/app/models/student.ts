import { Application } from './application';

/**
 * Matches the backend Student entity as serialised by the API.
 * Optional backend properties come back as explicit nulls, not omitted keys.
 */
export interface Student {
  id: number;
  fullName: string;
  email: string;
  phone: string | null;
  university: string | null;
  /** ISO 8601 date-time string. */
  createdAt: string;
  applications?: Application[];
}

/** Shape accepted by POST /api/Students and PUT /api/Students/{id}. */
export interface StudentRequest {
  fullName: string;
  email: string;
  phone?: string | null;
  university?: string | null;
}
