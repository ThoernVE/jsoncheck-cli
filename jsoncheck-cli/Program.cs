using System.CommandLine;
using JsonCheck.Services;
using JsonCheck.Utils;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Dynamic;


internal class Program
{
    static async Task<int> Main(string[] args)
    {
        var clipboardOption = new Option<bool>("--clipboard", aliases: new string[] { "-c" })
        {
            Description = "Read JSON from the clipboard"
        };

        var rootCommand = new RootCommand("Validate JSON input");
        rootCommand.Options.Add(clipboardOption);

        rootCommand.SetAction(parseResult =>
        {
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
            Console.WriteLine("Json is valid");
            return 0;
        }

        Console.WriteLine("JSON is invalid:");

        if (result.ErrorMessage is not null)
            Console.WriteLine($"Error : {result.ErrorMessage}");

        if (result.Path is not null)
            Console.WriteLine($"Path : {result.Path}");

        if (result.LineNumber is not null)
            Console.WriteLine($"Line : {result.LineNumber}");

        if (result.LinePosition is not null)
            Console.WriteLine($"Lineposition : {result.LinePosition}");

        return 1;
    }
}

internal sealed class InputContext
{
    public bool UseClipBoard { get; init; }
    public string? FilePath { get; init; }
    public bool HasStdin { get; init; }
}