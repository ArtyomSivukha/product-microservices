namespace UserManagement.Application.Exceptions;

public class ExternalServiceAvailabilityException : Exception
{
    public ExternalServiceAvailabilityException(string? message = null)
        : base(message ?? "External service is unavailable.")
    {
    }
}