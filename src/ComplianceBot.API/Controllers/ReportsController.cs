using ComplianceBot.Application.Reports.Commands.CreateReport;
using ComplianceBot.Application.Reports.Commands.DeleteReport;
using ComplianceBot.Application.Reports.Commands.SubmitReport;
using ComplianceBot.Application.Reports.Commands.UpdateReportStatus;
using ComplianceBot.Application.Reports.Queries.GetReport;
using ComplianceBot.Application.Reports.Queries.GetReports;
using ComplianceBot.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComplianceBot.API.Controllers;

/// <summary>
/// API controller for managing compliance reports
/// </summary>
[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all reports with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports(
        [FromQuery] ReportType? type,
        [FromQuery] ReportStatus? status,
        [FromQuery] string? reportingPeriod,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetReportsQuery
        {
            Type = type,
            Status = status,
            ReportingPeriod = reportingPeriod,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a report by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReport(Guid id)
    {
        var query = new GetReportQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new report
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetReport), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update report status
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateReportStatus(
        Guid id,
        [FromBody] UpdateReportStatusRequest request)
    {
        var command = new UpdateReportStatusCommand
        {
            ReportId = id,
            NewStatus = request.Status,
            Notes = request.Notes
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Submit report to regulatory authority
    /// </summary>
    [HttpPost("{id}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitReport(Guid id)
    {
        var command = new SubmitReportCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete a draft report
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReport(Guid id)
    {
        var command = new DeleteReportCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}

/// <summary>
/// Request model for updating report status
/// </summary>
public record UpdateReportStatusRequest
{
    public ReportStatus Status { get; init; }
    public string? Notes { get; init; }
}
