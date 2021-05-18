using Newtonsoft.Json;

namespace Verse.Models.Salute
{
    public abstract class SAction
    {
        [JsonProperty("type")]
        public abstract string Type { get;}
    }
    
    public class STextAction : SAction
    {
        public override string Type { get; } = "text";
        
        [JsonProperty("text")]
        public string Text { get; }
        
        [JsonProperty("should_send_to_backend")]
        public bool ShouldSendToBackend { get; set; } = true;

        public STextAction(string text)
        {
            Text = text;
        }
    }
}