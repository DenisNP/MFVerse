using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Verse.Models.Salute.Simple
{
    public class Button : TextBlock
    {
        [JsonProperty("actions")]
        public List<SAction> Actions { get; }

        [JsonProperty("type")]
        public string Type { get; set; } = "accept";

        [JsonProperty("style")]
        public string Style { get; set; } = "default";

        public Button(string text, params SAction[] actions) : base(text)
        {
            Actions = actions.ToList();
            Typeface = SaluteTextTypeface.button1;
        }
    }
}