namespace Turnify.Shared.Kernel.Abstractions;

public interface ICurrentTenant
{
    long? Id { get; }
    string? Slug { get; }
    bool IsAuthenticated { get; }
}
