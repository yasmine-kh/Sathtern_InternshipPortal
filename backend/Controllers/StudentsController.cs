using backend.Mapping;
using backend.Models.DTOs;
using backend.Models.Entities;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class StudentsController : ApiControllerBase
{
    private readonly IStudentService _students;

    public StudentsController(IStudentService students)
    {
        _students = students;
    }

    /// <summary>All registered students.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => FromResult(await _students.GetAllAsync(), s => s.ToDtos());

    /// <summary>A single student by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => FromResult(await _students.GetByIdAsync(id), s => s.ToDto());

    /// <summary>
    /// Looks up a single student by email address. Used by the frontend in
    /// place of downloading the whole student list.
    /// </summary>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
        => FromResult(await _students.GetByEmailAsync(email), s => s.ToDto());

    /// <summary>Registers a new student.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] Student student)
        => CreatedFromResult(await _students.CreateAsync(student), nameof(GetById), s => new { id = s.Id }, s => s.ToDto());

    /// <summary>Updates an existing student.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] Student student)
        => FromResult(await _students.UpdateAsync(id, student), s => s.ToDto());

    /// <summary>Deletes a student and their applications.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => FromResult(await _students.DeleteAsync(id));
}
