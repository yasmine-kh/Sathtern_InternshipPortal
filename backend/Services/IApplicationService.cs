using backend.Models.Entities;

namespace backend.Services;

public interface IApplicationService
{
    Task<ServiceResult<IReadOnlyList<Application>>> GetAllAsync();
    Task<ServiceResult<Application>> GetByIdAsync(int id);
    Task<ServiceResult<IReadOnlyList<Application>>> GetByStudentAsync(int studentId);
    Task<ServiceResult<IReadOnlyList<Application>>> GetByInternshipAsync(int internshipId);
    Task<ServiceResult<IReadOnlyList<Application>>> GetByStatusAsync(ApplicationStatus status);

    /// <summary>Submits an application, enforcing one per student per internship.</summary>
    Task<ServiceResult<Application>> ApplyAsync(int studentId, int internshipId);

    Task<ServiceResult<Application>> UpdateStatusAsync(int id, ApplicationStatus status);
    Task<ServiceResult> WithdrawAsync(int id);
}
