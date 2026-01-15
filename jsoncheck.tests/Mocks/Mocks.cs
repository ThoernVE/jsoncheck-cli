using JsonCheck.Utils;

internal sealed class MockClipboardReader : IClipboardReader
{
    public string? Value {get;set;}
    public string? ReadText() => Value;
}

internal sealed class MockFileReader : IFileReader
{
    public Dictionary<string, string> Files { get; } = new();
    public bool ThrowOnRead {get;set;}
    public bool Exists(string path) => Files.ContainsKey(path);
    public string ReadAllText(string path)
    {
        if(ThrowOnRead)
            throw new IOException("Error");

        return Files[path];
    }
}

internal sealed class MockStdinReader : IStdinReader
{
    public string Value {get;set;} = "";
    public string ReadStdin() => Value;
}