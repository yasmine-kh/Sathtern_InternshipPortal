using backend.Models.Entities;
using backend.Repositories;

namespace backend.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _students;

    public StudentService(IStudentRepository students)
    {
        _students = students;
    }

    public async Task<ServiceResult<IReadOnlyList<Student>>> GetAllAsync()
        => ServiceResult<IReadOnlyList<Student>>.Ok(await _students.GetAllAsync());

    public async Task<ServiceResult<Student>> GetByIdAsync(int id)
    {
        var student = await _students.GetByIdAsync(id);

        return student is null
            ? ServiceResult<Student>.NotFound($"Student {id} was not found.")
            : ServiceResult<Student>.Ok(student);
    }

    public async Task<ServiceResult<Student>> CreateAsync(Student student)
    {
        var validation = Validate(student);
        if (validation is not null)
        {
            return ServiceResult<Student>.Error(validation);
        }

        student.Email = student.Email.Trim();

        if (await _students.EmailExistsAsync(student.Email))
        {
            return ServiceResult<Student>.Conflict($"A student with email '{student.Email}' already exists.");
        }

        student.FullName = student.FullName.Trim();
        student.CreatedAt = DateTime.UtcNow;

        await _students.AddAsync(student);
        await _students.SaveChangesAsync();

        return ServiceResult<Student>.Ok(student);
    }

    public async Task<ServiceResult<Student>> UpdateAsync(int id, Student student)
    {
        var validation = Validate(student);
        if (validation is not null)
        {
            return ServiceResult<Student>.Error(validation);
        }

        var existing = await _students.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult<Student>.NotFound($"Student {id} was not found.");
        }

        var email = student.Email.Trim();

        if (await _students.EmailExistsAsync(email, excludeId: id))
        {
            return ServiceResult<Student>.Conflict($"Another student already uses email '{email}'.");
        }

        existing.FullName = student.FullName.Trim();
        existing.Email = email;
        existing.Phone = student.Phone;
        existing.University = student.University;

        _students.Update(existing);
        await _students.SaveChangesAsync();

        return ServiceResult<Student>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var existing = await _students.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult.NotFound($"Student {id} was not found.");
        }

        _students.Delete(existing);
        await _students.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    private static string? Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FullName))
        {
            return "Full name is required.";
        }

        if (string.IsNullOrWhiteSpace(student.Email))
        {
            return "Email is required.";
        }

        if (!student.Email.Contains('@'))
        {
            return "Email is not valid.";
        }

        return null;
    }
}
