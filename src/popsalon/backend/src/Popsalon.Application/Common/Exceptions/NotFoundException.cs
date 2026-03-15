namespace Popsalon.Application.Common.Exceptions;

public class NotFoundException(string entityName, object key)
    : Exception($"L'entité '{entityName}' avec la clé '{key}' est introuvable.");
