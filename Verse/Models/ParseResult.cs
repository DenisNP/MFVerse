namespace Verse.Models
{
    public class ParseResult
    {
        public Syllable[][] Syllables { get; set; } = null;
        public FootType FootType { get; set; } = FootType.Unknown;

        public static ParseResult Empty => new ParseResult();
    }
}