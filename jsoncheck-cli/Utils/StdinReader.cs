namespace JsonCheck.Utils
{
    public sealed class StdinReader : IStdinReader
    {
        public string ReadStdin() => Console.In.ReadToEnd();    
    }

    public interface IStdinReader
    {
        string ReadStdin();
    }
}
