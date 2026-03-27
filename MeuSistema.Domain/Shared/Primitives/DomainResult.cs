

namespace MeuSistema.Domain.Shared.Primitives;

internal sealed class DomainResult<T> 
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess { get; }

    private DomainResult(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private DomainResult(string error)
    {
        Error = error;
        IsSuccess = false;
    }

    public static DomainResult<T> Success(T value) => new(value);
    public static DomainResult<T> Failure(string error) => new(error);
}