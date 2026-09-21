using backend.Models.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class AdminController : ApiControllerBase
{
    private readonly IStudentService _students;
    private readonly IInternshipService _internships;
    private readonly IApplicationService _applications;

    public AdminController(
        IStudentService students,
        IInternshipService internships,
        IApplicationService applications)
    {
        _students = students;
        _internships = internships;
        _applications = applications;
    }

    /// <summary>
    /// Dashboard totals: students, internships, applications, and a breakdown
    /// of applications by status.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummary), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var students = await _students.GetAllAsync();
        if (!students.IsSuccess)
        {
            return FromResult(students);
        }

        var internships = await _internships.GetAllAsync();
        if (!internships.IsSuccess)
        {
            return FromResult(internships);
        }

        var applications = await _applications.GetAllAsync();
        if (!applications.IsSuccess)
        {
            return FromResult(applications);
        }

        var summary = DashboardSummary.Create(
            students.Value!.Count,
            internships.Value!.Count,
            applications.Value!);

        return Ok(summary);
    }
}
