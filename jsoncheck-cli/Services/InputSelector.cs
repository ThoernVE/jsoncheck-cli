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

        if (context.UseClipBoard)
            return ReadClipboard(out exitCode);

        if (context.HasStdin)
            return ReadStdin(out exitCode);

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

    private static string? ReadFile(out int exitCode)
    {
        exitCode = 0;

        return "Todo: implement FileReading"; //Todo
    }

    private static string? Fail(string message, out int exitCode)
    {
        Console.Error.WriteLine(message);
        exitCode = 2;
        return null;
    }
}