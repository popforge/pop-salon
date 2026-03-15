namespace Popsalon.Domain.Rules;

/// <summary>
/// Règle de domaine exécutée APRÈS la persistance d'une entité.
/// Utilisée pour les effets de bord (notifications, audit, etc.).
/// </summary>
public interface ISavedRule<TEntity> where TEntity : class
{
    Task ExecuteAsync(TEntity entity, CancellationToken ct = default);
}
