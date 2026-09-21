using backend.Data;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class InternshipRepository : Repository<Internship>, IInternshipRepository
{
    public InternshipRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Internship>> GetByCompanyAsync(string company)
        => await Set.AsNoTracking()
                    .Where(i => i.Company == company)
                    .OrderByDescending(i => i.PostedDate)
                    .ToListAsync();

    public async Task<IReadOnlyList<Internship>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return await GetAllAsync();
        }

        var pattern = $"%{term.Trim()}%";

        return await Set.AsNoTracking()
                        .Where(i => EF.Functions.Like(i.Title, pattern)
                                 || EF.Functions.Like(i.Company, pattern)
                                 || (i.Location != null && EF.Functions.Like(i.Location, pattern)))
                        .OrderByDescending(i => i.PostedDate)
                        .ToListAsync();
    }

    public async Task<Internship?> GetWithApplicationsAsync(int id)
        => await Set.Include(i => i.Applications)
                    .ThenInclude(a => a.Student)
                    .FirstOrDefaultAsync(i => i.Id == id);
}
