using backend.Data;
using backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(string email)
        => await Set.FirstOrDefaultAsync(s => s.Email == email);

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        => await Set.AnyAsync(s => s.Email == email && (excludeId == null || s.Id != excludeId));

    public async Task<Student?> GetWithApplicationsAsync(int id)
        => await Set.Include(s => s.Applications)
                    .ThenInclude(a => a.Internship)
                    .FirstOrDefaultAsync(s => s.Id == id);
}
