using backend.Models.Entities;

namespace backend.Services;

public interface IStudentService
{
    Task<ServiceResult<IReadOnlyList<Student>>> GetAllAsync();
    Task<ServiceResult<Student>> GetByIdAsync(int id);
    Task<ServiceResult<Student>> CreateAsync(Student student);
    Task<ServiceResult<Student>> UpdateAsync(int id, Student student);
    Task<ServiceResult> DeleteAsync(int id);
}
