using System.CommandLine;
using System.CommandLine.Parsing;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using JsonCheck.Utils;

internal static class InputSelector
{
    public static string? GetInput(InputContext context, out int exitCode)
    {
        exitCode = 0;

        int sourcesUsed = (context.UseClipBoard ? 1 : 0) + (!String.IsNullOrWhiteSpace(context.FilePath) ? 1 : 0) + (context.HasStdin ? 1 : 0);

        if (sourcesUsed == 0)
            return Fail("No input source specified. Use -c/--clipboard, -f/--file <path> or pipe input via stdin.", out exitCode);

        if (sourcesUsed > 1)
            return Fail("Multiple input sources specified. Please choose only one of: -c/--clipboard, -f/--file <path> or pipe input via stdin.", out exitCode);

        if (context.UseClipBoard)
            return ReadClipboard(out exitCode);

        if (context.HasStdin)
            return ReadStdin(out exitCode);

        if (!string.IsNullOrWhiteSpace(context.FilePath))
            return ReadFile(context.FilePath!, out exitCode);

        return Fail("No input source specified", out exitCode);
    }

    private static InputSource ResolveSource(ParseResult parseResult, Option<bool> clipboardOption)
    {
        if (parseResult.GetValue(clipboardOption))
            return InputSource.Clipboard;

        if (Console.IsInputRedirected)
            return InputSource.Stdin;

        return InputSource.None;
    }

    private static string? ReadClipboard(out int exitCode)
    {
        exitCode = 0;

        var clipboardText = new ClipboardReader().ReadText();
        if (string.IsNullOrWhiteSpace(clipboardText))
            return Fail("Clipboard is empty", out exitCode);

        return clipboardText;
    }

    private static string? ReadStdin(out int exitCode)
    {
        exitCode = 0;

        var stdinText = Console.In.ReadToEnd();
        if (string.IsNullOrWhiteSpace(stdinText))
            return Fail("Stdin input ios empty", out exitCode);

        return stdinText;
    }

    private static string? ReadFile(string path, out int exitCode)
    {
        exitCode = 0;

        try
        {
            if (!File.Exists(path))
                return Fail($"File not found: {path}", out exitCode);

            var text = File.ReadAllText(path);

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