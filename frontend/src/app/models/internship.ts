/**
 * Matches InternshipDto returned by the API.
 * Navigation properties are no longer serialised, so there is no
 * applications array here.
 */
export interface Internship {
  id: number;
  title: string;
  description: string | null;
  company: string;
  duration: string | null;
  location: string | null;
  /** ISO 8601 date-time string. */
  postedDate: string;
}

/** Matches InternshipSummary, nested inside an application response. */
export interface InternshipSummary {
  id: number;
  title: string;
  company: string;
}

/** Shape accepted by POST /api/Internships and PUT /api/Internships/{id}. */
export interface InternshipRequest {
  title: string;
  description?: string | null;
  company: string;
  duration?: string | null;
  location?: string | null;
}
