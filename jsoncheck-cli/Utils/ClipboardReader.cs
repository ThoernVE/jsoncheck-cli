using JsonCheck.Exceptions;
using TextCopy;

namespace JsonCheck.Utils
{
    public sealed class ClipboardReader : IClipboardReader
    {
        public string? ReadText() => ClipboardService.GetText();
    }
    public interface IClipboardReader
    {
        string? ReadText();
    }
}