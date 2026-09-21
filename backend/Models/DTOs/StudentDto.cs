namespace backend.Models.DTOs;

/// <summary>Student as returned by the API. Carries no navigation properties.</summary>
public class StudentDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? University { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>Minimal student reference embedded in an application response.</summary>
public class StudentSummary
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
