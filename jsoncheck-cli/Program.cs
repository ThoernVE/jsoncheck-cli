using System;
using System.Threading;
using JsonCheck.Services;
using JsonCheck.Utils;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var clipboardReader = new ClipboardReader();
        var clipboardText = clipboardReader.ReadText();

        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            Console.WriteLine("Clipboard Empty");
            Environment.Exit(1);
        }

        var jsonValidator = new JsonValidator();
        var result = jsonValidator.Validate(clipboardText);

        if (result.IsValid)
        {
            Console.WriteLine("JSON is valid");
            Environment.Exit(0);
        }

        Console.WriteLine("JSON is invalid");

        if (result.ErrorMessage is not null)
            Console.WriteLine($"Error : {result.ErrorMessage}");

        if (result.Path is not null)
            Console.WriteLine($"Path : {result.Path}");

        if (result.LineNumber is not null)
            Console.WriteLine($"Line : {result.LineNumber}:{result.LinePosition}");
        Environment.Exit(1);
    }
}