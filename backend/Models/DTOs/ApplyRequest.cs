using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTOs;

/// <summary>Body for submitting an application.</summary>
public class ApplyRequest
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int InternshipId { get; set; }
}
