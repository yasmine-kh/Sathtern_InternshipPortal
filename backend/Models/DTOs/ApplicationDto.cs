using backend.Models.Entities;

namespace backend.Models.DTOs;

/// <summary>
/// Application as returned by the API. The student and internship are flattened
/// to summaries, which breaks the Student -> Applications -> Student cycle that
/// the raw entities produce during serialisation.
/// </summary>
public class ApplicationDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int InternshipId { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime AppliedDate { get; set; }

    /// <summary>Present only when the endpoint loaded the student.</summary>
    public StudentSummary? Student { get; set; }

    /// <summary>Present only when the endpoint loaded the internship.</summary>
    public InternshipSummary? Internship { get; set; }
}
