using System.CommandLine;
using JsonCheck.Services;
using Spectre.Console;


internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var clipboardOption = new Option<bool>("--clipboard", aliases: new string[] { "-c" })
        {
            Description = "Read JSON from the clipboard"
        };

        var noColorOption = new Option<bool>("--no-color")
        {
            Description = "Disable colored output"
        };

        var rootCommand = new RootCommand("Validate JSON input");
        rootCommand.Options.Add(clipboardOption);
        rootCommand.Options.Add(noColorOption);

        rootCommand.SetAction(parseResult =>
        {
            if (parseResult.GetValue(noColorOption))
                DisableColorOutput();

            var context = new InputContext
            {
                UseClipBoard = parseResult.GetValue(clipboardOption),
                HasStdin = Console.IsInputRedirected,
                //TODO add filepath
            };

            var input = InputSelector.GetInput(context, out int exitCode);
            if (input is null)
                return exitCode;

            return ValidateAndReport(input);
        });

        ParseResult parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }

    private static int ValidateAndReport(string input)
    {
        var validator = new JsonValidator();
        var result = validator.Validate(input);

        if (result.IsValid)
        {
            AnsiConsole.MarkupLine("[green]Json is valid[/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[bold red]JSON is invalid:[/]");

        switch (result.ErrorKind)
        {
            case JsonErrorKind.NotJson:
                AnsiConsole.MarkupLine("[red]Input does not seem to be valid JSON, please check the input[/]");
                AnsiConsole.MarkupLine(Markup.Escape("Input did not start with '{' or '['"));
                break;
            case JsonErrorKind.SyntaxError:
                AnsiConsole.MarkupLine("[red]JSON syntax error[/]");
                AnsiConsole.MarkupLine($"[yellow]Message:[/] {result.ErrorMessage}");

                if (result.LineNumber is not null)
                    AnsiConsole.MarkupLine($"[yellow]Location: line {result.LineNumber}, column {result.LinePosition}[/]");

                if (result.Path is not null)
                    AnsiConsole.MarkupLine($"[yellow]Path: {result.Path}[/]");
                break;

            case JsonErrorKind.EmptyInput:
                AnsiConsole.MarkupLine("[yellow]Input is empty[/]");
                break;

            default:
                AnsiConsole.MarkupLine("[red]Unknown error[/]");
                AnsiConsole.MarkupLine($"[yellow]Message:[/] {result.ErrorMessage}");
                break;
        }

        return 1;
    }

    private static void DisableColorOutput()
    {
        AnsiConsole.Profile.Capabilities.Ansi = false;
        AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.NoColors;
    }
}

internal sealed class InputContext
{
    public bool UseClipBoard { get; init; }
    public string? FilePath { get; init; }
    public bool HasStdin { get; init; }
}