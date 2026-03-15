namespace Popsalon.Application.Common.Interfaces;

/// <summary>
/// Accès à l'utilisateur courant (injecté depuis le JWT via le contexte HTTP).
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
}
