namespace JsonCheck.Services;

public interface IJsonValidator
{

}

public sealed class JsonValidator : IJsonValidator
{

}

public sealed class JsonValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMesage { get; init; }
    public string? Path { get; init; }
    public int? LineNumber { get; init; }
    public int? LinePosition { get; init; }
}