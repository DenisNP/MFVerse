using Newtonsoft.Json;

namespace Verse.Models.Salute.Simple
{
    public class Suggestion
    {
        [JsonProperty("title")]
        public string Title { get; }

        [JsonProperty("action")]
        public STextAction Action { get; }

        public Suggestion(string text)
        {
            Title = text;
            Action = new STextAction(text);
        }
    }
}