/** Matches GET /api/Admin/dashboard. */
export interface DashboardSummary {
  totalStudents: number;
  totalInternships: number;
  totalApplications: number;
  /** Keyed by status name ("Pending" | "Accepted" | "Rejected"), always present. */
  applicationsByStatus: Record<string, number>;
}
