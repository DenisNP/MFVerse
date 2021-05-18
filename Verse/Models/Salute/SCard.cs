using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Verse.Models.Salute.Simple;

namespace Verse.Models.Salute
{
    public abstract class SCard
    {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }
    
    public class ListCard : SCard
    {
        public override string Type { get; } = "list_card";

        [JsonProperty("cells")]
        public List<SCell> Cells { get; }

        public ListCard(List<SCell> cells)
        {
            Cells = cells;
        }
    }

    public class GalleryCard : SCard
    {
        public override string Type { get; } = "gallery_card";

        [JsonProperty("items")]
        public List<SGalleryItem> Items { get; }

        public GalleryCard(params SGalleryItem[] items)
        {
            Items = items.ToList();
        }
    }

    public class GridCard : SCard
    {
        public override string Type { get; } = "grid_card";

        [JsonProperty("items")]
        public List<SGridItem> Items { get; }

        [JsonProperty("columns")]
        public int Columns { get; set; } = 1;

        [JsonProperty("item_width")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SizeType ItemWidth { get; set; } = SizeType.small;

        public GridCard(params SGridItem[] items)
        {
            Items = items.ToList();
        }
    }
}