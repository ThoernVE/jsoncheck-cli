using JsonCheck.Utils;

internal static class InputSelector
{
    public static string? GetInput(InputContext context, out int exitCode, IClipboardReader? clipboard = null,
        IFileReader? fileReader = null,
        IStdinReader? stdinReader = null)
    {
        exitCode = 0;

        clipboard ??= new ClipboardReader();
        fileReader ??= new FileReader();
        stdinReader ??= new StdinReader();

        int sourcesUsed = (context.UseClipBoard ? 1 : 0) + (!String.IsNullOrWhiteSpace(context.FilePath) ? 1 : 0) + (context.UseStdin ? 1 : 0);

        if (sourcesUsed == 0)
            return Fail("No input source specified. Use -c/--clipboard, -f/--file <path> or '-' for stdin.", out exitCode);

        if (sourcesUsed > 1)
            return Fail("Multiple input sources specified. Please choose only one of: -c/--clipboard, -f/--file <path> or '-' for stdin.", out exitCode);

        if (context.UseClipBoard)
            return ReadClipboard(clipboard, out exitCode);

        if (!string.IsNullOrWhiteSpace(context.FilePath))
            return ReadFile(context.FilePath!, fileReader, out exitCode);

        if (context.UseStdin)
            return ReadStdin(stdinReader, out exitCode);

        return Fail("No input source specified", out exitCode);
    }

    private static string? ReadClipboard(IClipboardReader clipboardReader, out int exitCode)
    {
        exitCode = 0;

        var clipboardText = clipboardReader.ReadText();
        if (string.IsNullOrWhiteSpace(clipboardText))
            return Fail("Clipboard is empty.", out exitCode);

        return clipboardText;
    }

    private static string? ReadStdin(IStdinReader stdin, out int exitCode)
    {
        exitCode = 0;

        var stdinText = stdin.ReadStdin();
        if (string.IsNullOrWhiteSpace(stdinText))
            return Fail("Stdin input is empty.", out exitCode);

        return stdinText;
    }

    private static string? ReadFile(string path, IFileReader fileReader, out int exitCode)
    {
        exitCode = 0;

        try
        {
            if (!fileReader.Exists(path))
                return Fail($"File not found: {path}", out exitCode);

            var text = fileReader.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(text))
                return Fail($"File is empty: {path}", out exitCode);

            return text;
        }
        catch (Exception ex)
        {
            return Fail($"Failed to read file '{path}': {ex.Message}", out exitCode);
        }
    }

    private static string? Fail(string message, out int exitCode)
    {
        Console.Error.WriteLine(message);
        exitCode = 2;
        return null;
    }
}