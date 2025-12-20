using JsonCheck.Exceptions;
using TextCopy;

namespace JsonCheck.Utils
{
    public interface IClipboardReader
    {
        string? ReadText();
    }
    public sealed class ClipboardReader : IClipboardReader
    {
        public string? ReadText() => ClipboardService.GetText();
    }
}