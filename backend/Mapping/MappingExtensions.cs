using backend.Models.DTOs;
using backend.Models.Entities;

namespace backend.Mapping;

/// <summary>
/// Entity -> DTO projections. Keeping these in one place means controllers
/// never hand raw entities (and their navigation properties) to the serialiser.
/// </summary>
public static class MappingExtensions
{
    public static StudentDto ToDto(this Student student) => new()
    {
        Id = student.Id,
        FullName = student.FullName,
        Email = student.Email,
        Phone = student.Phone,
        University = student.University,
        CreatedAt = student.CreatedAt
    };

    public static StudentSummary ToSummary(this Student student) => new()
    {
        Id = student.Id,
        FullName = student.FullName,
        Email = student.Email
    };

    public static InternshipDto ToDto(this Internship internship) => new()
    {
        Id = internship.Id,
        Title = internship.Title,
        Description = internship.Description,
        Company = internship.Company,
        Duration = internship.Duration,
        Location = internship.Location,
        PostedDate = internship.PostedDate
    };

    public static InternshipSummary ToSummary(this Internship internship) => new()
    {
        Id = internship.Id,
        Title = internship.Title,
        Company = internship.Company
    };

    public static ApplicationDto ToDto(this Application application) => new()
    {
        Id = application.Id,
        StudentId = application.StudentId,
        InternshipId = application.InternshipId,
        Status = application.Status,
        AppliedDate = application.AppliedDate,
        Student = application.Student?.ToSummary(),
        Internship = application.Internship?.ToSummary()
    };

    public static IReadOnlyList<StudentDto> ToDtos(this IEnumerable<Student> students)
        => students.Select(ToDto).ToList();

    public static IReadOnlyList<InternshipDto> ToDtos(this IEnumerable<Internship> internships)
        => internships.Select(ToDto).ToList();

    public static IReadOnlyList<ApplicationDto> ToDtos(this IEnumerable<Application> applications)
        => applications.Select(ToDto).ToList();
}
