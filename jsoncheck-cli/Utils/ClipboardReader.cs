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
        public string ReadText()
        {
            var text = ClipboardService.GetText() ?? throw new EmptyStringException("No text found in clipboard");
            return text;
        }

    }
}