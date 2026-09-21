namespace backend.Models.DTOs;

/// <summary>Internship as returned by the API. Carries no navigation properties.</summary>
public class InternshipDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Company { get; set; } = string.Empty;
    public string? Duration { get; set; }
    public string? Location { get; set; }
    public DateTime PostedDate { get; set; }
}

/// <summary>Minimal internship reference embedded in an application response.</summary>
public class InternshipSummary
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
}
