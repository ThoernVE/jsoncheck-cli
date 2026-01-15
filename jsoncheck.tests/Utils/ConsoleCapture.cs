internal sealed class StderrCapture : IDisposable
{
    private readonly TextWriter _original;
    private readonly StringWriter _writer;

    public StderrCapture()
    {
        _original = Console.Error;
        _writer = new StringWriter();
        Console.SetError(_writer);
    }

    public string Text => _writer.ToString();

    public void Dispose()
    {
        Console.SetError(_original);
        _writer.Dispose();
    }
}