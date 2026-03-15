// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using FluentValidation;
using MediatR;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Customers.Commands.Create;

public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone
) : IRequest<Guid>;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).EmailAddress().When(x => x.Email is not null).MaximumLength(255);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
    }
}

public class CreateCustomerCommandHandler(ICustomerRepository repository)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        var customer = new Customer(request.FirstName, request.LastName, request.Email, request.Phone);
        await repository.AddAsync(customer, ct);
        return customer.Id;
    }
}
