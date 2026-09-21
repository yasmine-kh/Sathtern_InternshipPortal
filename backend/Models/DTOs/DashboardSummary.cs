using backend.Models.Entities;

namespace backend.Models.DTOs;

/// <summary>Aggregate counts for the admin dashboard.</summary>
public class DashboardSummary
{
    public int TotalStudents { get; set; }
    public int TotalInternships { get; set; }
    public int TotalApplications { get; set; }

    /// <summary>Application counts keyed by status name, always including zeros.</summary>
    public Dictionary<string, int> ApplicationsByStatus { get; set; } = new();

    public static DashboardSummary Create(
        int students,
        int internships,
        IReadOnlyList<Application> applications)
    {
        var byStatus = Enum.GetValues<ApplicationStatus>()
                           .ToDictionary(s => s.ToString(), _ => 0);

        foreach (var application in applications)
        {
            byStatus[application.Status.ToString()]++;
        }

        return new DashboardSummary
        {
            TotalStudents = students,
            TotalInternships = internships,
            TotalApplications = applications.Count,
            ApplicationsByStatus = byStatus
        };
    }
}
