// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Microsoft.EntityFrameworkCore;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Application.EntityViews;

namespace Popsalon.Application.Features.Customers.Queries.GetAll;

public record GetAllCustomersQuery : IRequest<IQueryable<CustomerView>>;

public class GetAllCustomersQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllCustomersQuery, IQueryable<CustomerView>>
{
    public Task<IQueryable<CustomerView>> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        var query = db.Customers
            .AsNoTracking()
            .Select(c => new CustomerView
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                FullName = c.FirstName + " " + c.LastName,
                Email = c.Email,
                Phone = c.Phone,
            });

        return Task.FromResult(query);
    }
}
