using backend.Models.Entities;
using backend.Repositories;

namespace backend.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applications;
    private readonly IStudentRepository _students;
    private readonly IInternshipRepository _internships;

    public ApplicationService(
        IApplicationRepository applications,
        IStudentRepository students,
        IInternshipRepository internships)
    {
        _applications = applications;
        _students = students;
        _internships = internships;
    }

    public async Task<ServiceResult<IReadOnlyList<Application>>> GetAllAsync()
        => ServiceResult<IReadOnlyList<Application>>.Ok(await _applications.GetAllAsync());

    public async Task<ServiceResult<Application>> GetByIdAsync(int id)
    {
        var application = await _applications.GetWithDetailsAsync(id);

        return application is null
            ? ServiceResult<Application>.NotFound($"Application {id} was not found.")
            : ServiceResult<Application>.Ok(application);
    }

    public async Task<ServiceResult<IReadOnlyList<Application>>> GetByStudentAsync(int studentId)
    {
        if (await _students.GetByIdAsync(studentId) is null)
        {
            return ServiceResult<IReadOnlyList<Application>>.NotFound($"Student {studentId} was not found.");
        }

        return ServiceResult<IReadOnlyList<Application>>.Ok(await _applications.GetByStudentAsync(studentId));
    }

    public async Task<ServiceResult<IReadOnlyList<Application>>> GetByInternshipAsync(int internshipId)
    {
        if (await _internships.GetByIdAsync(internshipId) is null)
        {
            return ServiceResult<IReadOnlyList<Application>>.NotFound($"Internship {internshipId} was not found.");
        }

        return ServiceResult<IReadOnlyList<Application>>.Ok(await _applications.GetByInternshipAsync(internshipId));
    }

    public async Task<ServiceResult<IReadOnlyList<Application>>> GetByStatusAsync(ApplicationStatus status)
        => ServiceResult<IReadOnlyList<Application>>.Ok(await _applications.GetByStatusAsync(status));

    public async Task<ServiceResult<Application>> ApplyAsync(int studentId, int internshipId)
    {
        if (await _students.GetByIdAsync(studentId) is null)
        {
            return ServiceResult<Application>.NotFound($"Student {studentId} was not found.");
        }

        if (await _internships.GetByIdAsync(internshipId) is null)
        {
            return ServiceResult<Application>.NotFound($"Internship {internshipId} was not found.");
        }

        // Checked up front so the caller gets a clean conflict rather than a
        // unique-index violation from the database.
        if (await _applications.HasAppliedAsync(studentId, internshipId))
        {
            return ServiceResult<Application>.Conflict(
                $"Student {studentId} has already applied to internship {internshipId}.");
        }

        var application = new Application
        {
            StudentId = studentId,
            InternshipId = internshipId,
            Status = ApplicationStatus.Pending,
            AppliedDate = DateTime.UtcNow
        };

        await _applications.AddAsync(application);
        await _applications.SaveChangesAsync();

        return ServiceResult<Application>.Ok(application);
    }

    public async Task<ServiceResult<Application>> UpdateStatusAsync(int id, ApplicationStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            return ServiceResult<Application>.Error($"'{status}' is not a valid application status.");
        }

        var existing = await _applications.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult<Application>.NotFound($"Application {id} was not found.");
        }

        if (existing.Status == status)
        {
            return ServiceResult<Application>.Conflict($"Application {id} is already {status}.");
        }

        existing.Status = status;

        _applications.Update(existing);
        await _applications.SaveChangesAsync();

        return ServiceResult<Application>.Ok(existing);
    }

    public async Task<ServiceResult> WithdrawAsync(int id)
    {
        var existing = await _applications.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult.NotFound($"Application {id} was not found.");
        }

        _applications.Delete(existing);
        await _applications.SaveChangesAsync();

        return ServiceResult.Ok();
    }
}
