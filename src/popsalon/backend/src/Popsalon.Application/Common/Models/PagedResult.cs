namespace Popsalon.Application.Common.Models;

/// <summary>Résultat paginé générique pour les requêtes OData / liste.</summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount)
{
    public static PagedResult<T> Empty => new([], 0);
}
