using Newtonsoft.Json;
using Verse.Models.Salute.Simple;

namespace Verse.Models.Salute
{
    public class SGridItem
    {
        [JsonProperty("type")]
        public string Type { get; } = "greeting_grid_item";

        [JsonProperty("top_text")]
        public TextBlock TopText { get; }
        
        [JsonProperty("bottom_text")]
        public TextBlock BottomText { get; }

        [JsonProperty("paddings")]
        public Edges Paddings { get; set; }

        [JsonProperty("actions")]
        public SAction[] Actions { get; set; }
        
        public SGridItem(TextBlock topText, TextBlock bottomText)
        {
            TopText = topText;
            BottomText = bottomText;
        }
    }
}