using FluentValidation;
using MediatR;

namespace Popsalon.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior : exécute FluentValidation avant chaque Command.
/// Lève une ValidationException si des règles sont violées.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct))))
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .ToList();

        if (failures.Count > 0)
            throw new FluentValidation.ValidationException(failures);

        return await next();
    }
}
