namespace backend.Models.Entities;

public class Application
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public int InternshipId { get; set; }
    public Internship? Internship { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
}
