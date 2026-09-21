import { Application } from './application';

/** Matches the backend Internship entity as serialised by the API. */
export interface Internship {
  id: number;
  title: string;
  description: string | null;
  company: string;
  duration: string | null;
  location: string | null;
  /** ISO 8601 date-time string. */
  postedDate: string;
  applications?: Application[];
}

/** Shape accepted by POST /api/Internships and PUT /api/Internships/{id}. */
export interface InternshipRequest {
  title: string;
  description?: string | null;
  company: string;
  duration?: string | null;
  location?: string | null;
}
