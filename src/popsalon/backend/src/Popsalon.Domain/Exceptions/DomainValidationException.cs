namespace Popsalon.Domain.Exceptions;

public class DomainValidationException(string message) : Exception(message);
