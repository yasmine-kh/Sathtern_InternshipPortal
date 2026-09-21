using System.ComponentModel.DataAnnotations;

namespace backend.Models.Entities;

public class Internship
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(150)]
    public string Company { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Duration { get; set; }

    [MaxLength(150)]
    public string? Location { get; set; }

    public DateTime PostedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
