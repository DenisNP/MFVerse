using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Verse.Models.Salute
{
    public abstract class SItem { }

    public class BubbleItem : SItem
    {
        [JsonProperty("bubble")]
        public Bubble Bubble { get; }

        public BubbleItem(Bubble bubble)
        {
            Bubble = bubble;
        }

        public BubbleItem(string text)
        {
            Bubble = new Bubble(text);
        }
    }

    public class Bubble
    {
        private readonly Regex _tagsRegex = new("<[^>]*>");
        
        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("markdown")]
        public bool Markdown { get; set; } = true;

        [JsonProperty("expand_policy")]
        public string ExpandPolicy { get; set; } = "force_expand";

        public Bubble(string text)
        {
            Text = _tagsRegex.Replace(text, "").Replace("'", "").Replace("*", "");
        }
    }

    public class GridCardItem : SItem
    {
        [JsonProperty("card")]
        public GridCard Card { get; }

        public GridCardItem(GridCard card)
        {
            Card = card;
        }
    }
    
    public class GalleryItem : SItem
    {
        [JsonProperty("card")]
        public GalleryCard Card { get; }

        public GalleryItem(GalleryCard card)
        {
            Card = card;
        }
    }

    public class ListCardItem : SItem
    {
        [JsonProperty("card")]
        public ListCard Card { get; }

        public ListCardItem(ListCard card)
        {
            Card = card;
        }
    }

    public class CommandItem : SItem
    {
        [JsonProperty("command")]
        public SCommand Command { get; }

        public CommandItem(SCommand command)
        {
            Command = command;
        }
    }
}