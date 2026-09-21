using backend.Data;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Includes the student and internship, so callers listing every
    /// application (the admin view) get the same embedded summaries the
    /// filtered queries return rather than bare ids.
    /// </summary>
    public override async Task<IReadOnlyList<Application>> GetAllAsync()
        => await Set.AsNoTracking()
                    .Include(a => a.Student)
                    .Include(a => a.Internship)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

    public async Task<bool> HasAppliedAsync(int studentId, int internshipId)
        => await Set.AnyAsync(a => a.StudentId == studentId && a.InternshipId == internshipId);

    public async Task<IReadOnlyList<Application>> GetByStudentAsync(int studentId)
        => await Set.AsNoTracking()
                    .Include(a => a.Internship)
                    .Where(a => a.StudentId == studentId)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

    public async Task<IReadOnlyList<Application>> GetByInternshipAsync(int internshipId)
        => await Set.AsNoTracking()
                    .Include(a => a.Student)
                    .Where(a => a.InternshipId == internshipId)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

    public async Task<IReadOnlyList<Application>> GetByStatusAsync(ApplicationStatus status)
        => await Set.AsNoTracking()
                    .Include(a => a.Student)
                    .Include(a => a.Internship)
                    .Where(a => a.Status == status)
                    .OrderByDescending(a => a.AppliedDate)
                    .ToListAsync();

    public async Task<Application?> GetWithDetailsAsync(int id)
        => await Set.Include(a => a.Student)
                    .Include(a => a.Internship)
                    .FirstOrDefaultAsync(a => a.Id == id);
}
