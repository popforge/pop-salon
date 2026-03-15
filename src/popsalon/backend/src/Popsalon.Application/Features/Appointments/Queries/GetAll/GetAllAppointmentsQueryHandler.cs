// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Microsoft.EntityFrameworkCore;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Application.EntityViews;

namespace Popsalon.Application.Features.Appointments.Queries.GetAll;

public class GetAllAppointmentsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllAppointmentsQuery, IQueryable<AppointmentView>>
{
    public Task<IQueryable<AppointmentView>> Handle(GetAllAppointmentsQuery request, CancellationToken ct)
    {
        var query = db.Appointments
            .AsNoTracking()
            .Select(a => new AppointmentView
            {
                Id = a.Id,
                Date = a.Date,
                Notes = a.Notes,
                CustomerId = a.CustomerId,
                CustomerFullName = a.Customer.FirstName + " " + a.Customer.LastName,
                CustomerEmail = a.Customer.Email,
            });

        return Task.FromResult(query);
    }
}
