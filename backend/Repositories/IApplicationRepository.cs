using backend.Models.Entities;

namespace backend.Repositories;

public interface IApplicationRepository : IRepository<Application>
{
    /// <summary>
    /// True when this student has already applied to this internship.
    /// Mirrors the composite unique index on (StudentId, InternshipId) so the
    /// service layer can return a clean conflict instead of a DB exception.
    /// </summary>
    Task<bool> HasAppliedAsync(int studentId, int internshipId);

    Task<IReadOnlyList<Application>> GetByStudentAsync(int studentId);

    Task<IReadOnlyList<Application>> GetByInternshipAsync(int internshipId);

    Task<IReadOnlyList<Application>> GetByStatusAsync(ApplicationStatus status);

    /// <summary>Loads the application with its Student and Internship attached.</summary>
    Task<Application?> GetWithDetailsAsync(int id);
}
