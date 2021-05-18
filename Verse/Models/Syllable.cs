namespace Verse.Models
{
    public class Syllable
    {
        public SyllableType Type { get; set; }
        public string Text { get; set; }
    }

    public enum SyllableType
    {
        Consonant,
        Stressed,
        Unstressed
    }
}