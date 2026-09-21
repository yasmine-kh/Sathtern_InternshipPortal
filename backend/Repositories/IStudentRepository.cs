using backend.Models.Entities;

namespace backend.Repositories;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);

    /// <summary>
    /// True when another student already uses this email.
    /// <paramref name="excludeId"/> lets an update ignore the row being edited.
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    Task<Student?> GetWithApplicationsAsync(int id);
}
