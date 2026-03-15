// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Microsoft.EntityFrameworkCore;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Application.EntityViews;
using Popsalon.Domain.Entities;

namespace Popsalon.Application.Features.Customers.Queries.GetById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerView>;

public class GetCustomerByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCustomerByIdQuery, CustomerView>
{
    public async Task<CustomerView> Handle(GetCustomerByIdQuery request, CancellationToken ct)
    {
        var view = await db.Customers
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CustomerView
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                FullName = c.FirstName + " " + c.LastName,
                Email = c.Email,
                Phone = c.Phone,
            })
            .FirstOrDefaultAsync(ct);

        return view ?? throw new NotFoundException(nameof(Customer), request.Id);
    }
}
