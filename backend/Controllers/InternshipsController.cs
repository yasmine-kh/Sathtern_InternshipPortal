using backend.Models.Entities;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public class InternshipsController : ApiControllerBase
{
    private readonly IInternshipService _internships;

    public InternshipsController(IInternshipService internships)
    {
        _internships = internships;
    }

    /// <summary>
    /// Lists internships. Optionally filtered by a free-text search across
    /// title, company and location.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Internship>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
        => FromResult(string.IsNullOrWhiteSpace(search)
            ? await _internships.GetAllAsync()
            : await _internships.SearchAsync(search));

    /// <summary>A single internship by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Internship), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => FromResult(await _internships.GetByIdAsync(id));

    /// <summary>Posts a new internship.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Internship), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] Internship internship)
        => CreatedFromResult(await _internships.CreateAsync(internship), nameof(GetById), i => new { id = i.Id });

    /// <summary>Updates an existing internship.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Internship), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] Internship internship)
        => FromResult(await _internships.UpdateAsync(id, internship));

    /// <summary>Deletes an internship and its applications.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => FromResult(await _internships.DeleteAsync(id));
}
