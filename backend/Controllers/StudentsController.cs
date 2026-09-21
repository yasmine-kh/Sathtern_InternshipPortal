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
    [ProducesResponseType(typeof(IReadOnlyList<Student>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => FromResult(await _students.GetAllAsync());

    /// <summary>A single student by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => FromResult(await _students.GetByIdAsync(id));

    /// <summary>Registers a new student.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Student), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] Student student)
        => CreatedFromResult(await _students.CreateAsync(student), nameof(GetById), s => new { id = s.Id });

    /// <summary>Updates an existing student.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] Student student)
        => FromResult(await _students.UpdateAsync(id, student));

    /// <summary>Deletes a student and their applications.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => FromResult(await _students.DeleteAsync(id));
}
