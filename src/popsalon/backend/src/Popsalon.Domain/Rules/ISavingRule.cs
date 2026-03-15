namespace Popsalon.Domain.Rules;

/// <summary>
/// Règle de domaine exécutée AVANT la persistance d'une entité.
/// Lève une DomainValidationException si la règle n'est pas respectée.
/// </summary>
public interface ISavingRule<TEntity> where TEntity : class
{
    Task ValidateAsync(TEntity entity, CancellationToken ct = default);
}
