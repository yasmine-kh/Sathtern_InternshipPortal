using backend.Models.Entities;

namespace backend.Repositories;

public interface IInternshipRepository : IRepository<Internship>
{
    Task<IReadOnlyList<Internship>> GetByCompanyAsync(string company);

    /// <summary>Case-insensitive match on title, company or location.</summary>
    Task<IReadOnlyList<Internship>> SearchAsync(string term);

    Task<Internship?> GetWithApplicationsAsync(int id);
}
