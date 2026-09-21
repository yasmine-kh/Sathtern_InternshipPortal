using backend.Models.Entities;

namespace backend.Services;

public interface IInternshipService
{
    Task<ServiceResult<IReadOnlyList<Internship>>> GetAllAsync();
    Task<ServiceResult<IReadOnlyList<Internship>>> SearchAsync(string term);
    Task<ServiceResult<Internship>> GetByIdAsync(int id);
    Task<ServiceResult<Internship>> CreateAsync(Internship internship);
    Task<ServiceResult<Internship>> UpdateAsync(int id, Internship internship);
    Task<ServiceResult> DeleteAsync(int id);
}
