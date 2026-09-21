using backend.Mapping;
using backend.Models.DTOs;
using backend.Models.Entities;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class ApplicationsController : ApiControllerBase
{
    private readonly IApplicationService _applications;

    public ApplicationsController(IApplicationService applications)
    {
        _applications = applications;
    }

    /// <summary>All applications, optionally filtered by status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status)
        => FromResult(
            status.HasValue
                ? await _applications.GetByStatusAsync(status.Value)
                : await _applications.GetAllAsync(),
            a => a.ToDtos());

    /// <summary>A single application, with its student and internship.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => FromResult(await _applications.GetByIdAsync(id), a => a.ToDto());

    /// <summary>Applications submitted by one student ("my applications").</summary>
    [HttpGet("student/{studentId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudent(int studentId)
        => FromResult(await _applications.GetByStudentAsync(studentId), a => a.ToDtos());

    /// <summary>Applications received for one internship (admin view).</summary>
    [HttpGet("internship/{internshipId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByInternship(int internshipId)
        => FromResult(await _applications.GetByInternshipAsync(internshipId), a => a.ToDtos());

    /// <summary>
    /// Applies a student to an internship. Returns 409 if that student has
    /// already applied to it.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Apply([FromBody] ApplyRequest request)
        => CreatedFromResult(
            await _applications.ApplyAsync(request.StudentId, request.InternshipId),
            nameof(GetById),
            a => new { id = a.Id },
            a => a.ToDto());

    /// <summary>Approves or rejects an application.</summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        => FromResult(await _applications.UpdateStatusAsync(id, request.Status), a => a.ToDto());

    /// <summary>Withdraws (deletes) an application.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(int id)
        => FromResult(await _applications.WithdrawAsync(id));
}
