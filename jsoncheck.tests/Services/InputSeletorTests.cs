using Xunit;

public class InputSelectorTests
{
    [Fact]
    public void GetInput_NoSourceSpecified_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = null,
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: new MockFileReader(),
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("No input source specified", err.Text);
    }

    [Fact]
    public void GetInput_MultipleSourcesSpecified_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = true,
            FilePath = "a.json",
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader { Value = "{\"a\":1}" },
            fileReader: new MockFileReader(),
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("Multiple input sources", err.Text);
    }

    [Fact]
    public void GetInput_ClipboardSelected_ReturnsClipboardText_ExitCode0()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = true,
            FilePath = null,
            UseStdin = false
        };

        var clipboard = new MockClipboardReader { Value = "{\"a\":1}" };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: clipboard,
            fileReader: new MockFileReader(),
            stdinReader: new MockStdinReader());

        Assert.Equal("{\"a\":1}", result);
        Assert.Equal(0, exitCode);
        Assert.Equal("", err.Text);
    }

    [Fact]
    public void GetInput_ClipboardSelectedButEmpty_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = true,
            FilePath = null,
            UseStdin = false
        };

        var clipboard = new MockClipboardReader { Value = "   " };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: clipboard,
            fileReader: new MockFileReader(),
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("Clipboard is empty", err.Text);
    }

    [Fact]
    public void GetInput_FileSelected_ReturnsFileText_ExitCode0()
    {
        using var err = new StderrCapture();

        var fileReader = new MockFileReader();
        fileReader.Files["test.json"] = "{\"x\":2}";

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = "test.json",
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: fileReader,
            stdinReader: new MockStdinReader());

        Assert.Equal("{\"x\":2}", result);
        Assert.Equal(0, exitCode);
        Assert.Equal("", err.Text);
    }

    [Fact]
    public void GetInput_FileSelectedButNotFound_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = "missing.json",
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: new MockFileReader(),
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("File not found", err.Text);
    }

    [Fact]
    public void GetInput_FileSelectedButEmpty_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var fileReader = new MockFileReader();
        fileReader.Files["empty.json"] = "   ";

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = "empty.json",
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: fileReader,
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("File is empty", err.Text);
    }

    [Fact]
    public void GetInput_FileReadThrows_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var fileReader = new MockFileReader();
        fileReader.Files["boom.json"] = "{\"a\":1}";
        fileReader.ThrowOnRead = true;

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = "boom.json",
            UseStdin = false
        };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: fileReader,
            stdinReader: new MockStdinReader());

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("Failed to read file", err.Text);
    }

    [Fact]
    public void GetInput_StdinSelected_ReturnsStdinText_ExitCode0()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = null,
            UseStdin = true
        };

        var stdin = new MockStdinReader { Value = "{\"stdin\":true}" };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: new MockFileReader(),
            stdinReader: stdin);

        Assert.Equal("{\"stdin\":true}", result);
        Assert.Equal(0, exitCode);
        Assert.Equal("", err.Text);
    }

    [Fact]
    public void GetInput_StdinSelectedButEmpty_ReturnsNull_ExitCode2()
    {
        using var err = new StderrCapture();

        var ctx = new InputContext
        {
            UseClipBoard = false,
            FilePath = null,
            UseStdin = true
        };

        var stdin = new MockStdinReader { Value = " " };

        var result = InputSelector.GetInput(ctx, out var exitCode,
            clipboard: new MockClipboardReader(),
            fileReader: new MockFileReader(),
            stdinReader: stdin);

        Assert.Null(result);
        Assert.Equal(2, exitCode);
        Assert.Contains("Stdin input is empty", err.Text);
    }
}