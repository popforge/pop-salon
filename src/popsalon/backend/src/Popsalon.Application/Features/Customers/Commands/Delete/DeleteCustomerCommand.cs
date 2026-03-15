// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Customers.Commands.Delete;

public record DeleteCustomerCommand(Guid Id) : IRequest;

public class DeleteCustomerCommandHandler(ICustomerRepository repository)
    : IRequestHandler<DeleteCustomerCommand>
{
    public async Task Handle(DeleteCustomerCommand request, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Customer), request.Id);

        await repository.DeleteAsync(customer, ct);
    }
}
