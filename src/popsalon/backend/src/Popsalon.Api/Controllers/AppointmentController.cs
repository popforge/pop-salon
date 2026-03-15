// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Appointment.yml

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Popsalon.Application.EntityViews;
using Popsalon.Application.Features.Appointments.Commands.Create;
using Popsalon.Application.Features.Appointments.Commands.Delete;
using Popsalon.Application.Features.Appointments.Commands.Update;
using Popsalon.Application.Features.Appointments.Queries.GetAll;
using Popsalon.Application.Features.Appointments.Queries.GetById;

namespace Popsalon.Api.Controllers;

[Route("api/v1/appointments")]
public partial class AppointmentController(ISender mediator) : ODataController
{
    [HttpGet]
    [EnableQuery(MaxTop = 200, AllowedQueryOptions = AllowedQueryOptions.All)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllAppointmentsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [EnableQuery]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAppointmentByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentCommand command, CancellationToken ct)
    {
        await mediator.Send(command with { Id = id }, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAppointmentCommand(id), ct);
        return NoContent();
    }
}
