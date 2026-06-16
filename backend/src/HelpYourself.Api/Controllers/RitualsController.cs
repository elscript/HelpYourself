using HelpYourself.Core.DTOs;
using HelpYourself.Core.Enums;
using HelpYourself.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpYourself.Api.Controllers;

[ApiController]
[Route("api/rituals")]
public sealed class RitualsController : ControllerBase
{
    private readonly IRitualService _ritualService;

    public RitualsController(IRitualService ritualService)
    {
        _ritualService = ritualService;
    }

    /// <summary>Generate a new personalized ritual via LLM.</summary>
    [HttpPost("generate")]
    [ProducesResponseType<GenerateRitualResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(
        [FromBody] GenerateRitualRequest request,
        CancellationToken ct)
    {
        var result = await _ritualService.GenerateAsync(request, ct);
        return Ok(result);
    }

    /// <summary>Record user feedback for a completed ritual.</summary>
    [HttpPost("{ritualId:guid}/feedback")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Feedback(
        Guid ritualId,
        [FromBody] FeedbackRequest request,
        CancellationToken ct)
    {
        await _ritualService.RecordFeedbackAsync(ritualId, request, ct);
        return NoContent();
    }

    /// <summary>Return a cached instant ritual from the First-Aid Kit.</summary>
    [HttpPost("instant/{archetype}")]
    [ProducesResponseType<GenerateRitualResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Instant(
        Archetype archetype,
        CancellationToken ct)
    {
        if (!Enum.IsDefined(archetype))
            return BadRequest($"Unknown archetype '{archetype}'.");

        var result = await _ritualService.GetInstantRitualAsync(archetype, ct);
        return Ok(result);
    }
}
