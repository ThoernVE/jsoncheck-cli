using System;
using System.Threading;
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
        }

        Console.WriteLine("Clipboard read correctly");
        Console.WriteLine(clipboardText);
    }
}