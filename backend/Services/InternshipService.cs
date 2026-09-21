using backend.Models.Entities;
using backend.Repositories;

namespace backend.Services;

public class InternshipService : IInternshipService
{
    private readonly IInternshipRepository _internships;

    public InternshipService(IInternshipRepository internships)
    {
        _internships = internships;
    }

    public async Task<ServiceResult<IReadOnlyList<Internship>>> GetAllAsync()
        => ServiceResult<IReadOnlyList<Internship>>.Ok(await _internships.GetAllAsync());

    public async Task<ServiceResult<IReadOnlyList<Internship>>> SearchAsync(string term)
        => ServiceResult<IReadOnlyList<Internship>>.Ok(await _internships.SearchAsync(term));

    public async Task<ServiceResult<Internship>> GetByIdAsync(int id)
    {
        var internship = await _internships.GetByIdAsync(id);

        return internship is null
            ? ServiceResult<Internship>.NotFound($"Internship {id} was not found.")
            : ServiceResult<Internship>.Ok(internship);
    }

    public async Task<ServiceResult<Internship>> CreateAsync(Internship internship)
    {
        var validation = Validate(internship);
        if (validation is not null)
        {
            return ServiceResult<Internship>.Error(validation);
        }

        internship.Title = internship.Title.Trim();
        internship.Company = internship.Company.Trim();
        internship.PostedDate = DateTime.UtcNow;

        await _internships.AddAsync(internship);
        await _internships.SaveChangesAsync();

        return ServiceResult<Internship>.Ok(internship);
    }

    public async Task<ServiceResult<Internship>> UpdateAsync(int id, Internship internship)
    {
        var validation = Validate(internship);
        if (validation is not null)
        {
            return ServiceResult<Internship>.Error(validation);
        }

        var existing = await _internships.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult<Internship>.NotFound($"Internship {id} was not found.");
        }

        existing.Title = internship.Title.Trim();
        existing.Description = internship.Description;
        existing.Company = internship.Company.Trim();
        existing.Duration = internship.Duration;
        existing.Location = internship.Location;

        _internships.Update(existing);
        await _internships.SaveChangesAsync();

        return ServiceResult<Internship>.Ok(existing);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var existing = await _internships.GetByIdAsync(id);
        if (existing is null)
        {
            return ServiceResult.NotFound($"Internship {id} was not found.");
        }

        _internships.Delete(existing);
        await _internships.SaveChangesAsync();

        return ServiceResult.Ok();
    }

    private static string? Validate(Internship internship)
    {
        if (string.IsNullOrWhiteSpace(internship.Title))
        {
            return "Title is required.";
        }

        if (string.IsNullOrWhiteSpace(internship.Company))
        {
            return "Company is required.";
        }

        return null;
    }
}
