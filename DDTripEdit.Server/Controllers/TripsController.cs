using DDTripEdit.Server.Services;
using DDTripEdit.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace DDTripEdit.Server.Controllers;

/// <summary>API controller for managing trips.</summary>
[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly CsvService _csvService;

    /// <summary>Initializes a new instance of <see cref="TripsController"/>.</summary>
    public TripsController(CsvService csvService)
    {
        _csvService = csvService;
    }

    /// <summary>Gets all trips.</summary>
    [HttpGet]
    public ActionResult<List<Trip>> GetAll()
    {
        var trips = _csvService.ReadAll();
        return Ok(trips);
    }

    /// <summary>Gets a specific trip by id.</summary>
    [HttpGet("{id:int}")]
    public ActionResult<Trip> GetById(int id)
    {
        var trip = _csvService.GetById(id);
        if (trip is null)
            return NotFound($"Trip with id {id} not found.");
        return Ok(trip);
    }

    /// <summary>Creates a new trip.</summary>
    [HttpPost]
    public ActionResult<Trip> Create([FromBody] Trip trip)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _csvService.Add(trip);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing trip.</summary>
    [HttpPut("{id:int}")]
    public ActionResult<Trip> Update(int id, [FromBody] Trip trip)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = _csvService.Update(id, trip);
        if (!success)
            return NotFound($"Trip with id {id} not found.");

        return Ok(trip);
    }

    /// <summary>Deletes a trip.</summary>
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var success = _csvService.Delete(id);
        if (!success)
            return NotFound($"Trip with id {id} not found.");

        return NoContent();
    }
}
