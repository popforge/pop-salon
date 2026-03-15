// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using FluentValidation;
using MediatR;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Customers.Commands.Update;

public record UpdateCustomerCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone
) : IRequest;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null).MaximumLength(255);
    }
}

public class UpdateCustomerCommandHandler(ICustomerRepository repository)
    : IRequestHandler<UpdateCustomerCommand>
{
    public async Task Handle(UpdateCustomerCommand request, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Customer), request.Id);

        customer.Update(request.FirstName, request.LastName, request.Email, request.Phone);
        await repository.UpdateAsync(customer, ct);
    }
}
