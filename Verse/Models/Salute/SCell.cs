using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Verse.Models.Salute.Simple;

namespace Verse.Models.Salute
{
    public abstract class SCell
    {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    public class LrvCell : SCell
    {
        public override string Type { get; } = "left_right_cell_view";

        [JsonProperty("paddings")]
        public Edges Paddings { get; set; }
        
        [JsonProperty("left")]
        public LrvCellLeft Left { get; set; } = new();
        
        [JsonProperty("right")]
        public LrvCellRight Right { get; set; } = new();

        [JsonProperty("divider")]
        public CellDivider Divider { get; set; }

        [JsonProperty("actions")]
        public SAction[] Actions { get; set; }
    }

    public class LrvCellLeft
    {
        [JsonProperty("type")]
        public string Type { get; } = "simple_left_view";

        [JsonProperty("icon")]
        public SIcon Icon { get; set; }

        [JsonProperty("icon_vertical_gravity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public VerticalGravity IconVerticalGravity { get; set; } = VerticalGravity.center;

        [JsonProperty("texts")]
        public LrvCellTexts Texts { get; set; }
    }

    public class LrvCellTexts
    {
        [JsonProperty("title")]
        public TextBlock Title { get; set; }

        [JsonProperty("subtitle")]
        public TextBlock Subtitle { get; set; }
    }

    public class LrvCellRight
    {
        [JsonProperty("type")]
        public string Type { get; } = "detail_right_view";
        
        [JsonProperty("detail")]
        public TextBlock Detail { get; set; }

        [JsonProperty("vertical_gravity")]
        [JsonConverter(typeof(StringEnumConverter))]
        public VerticalGravity VerticalGravity { get; set; } = VerticalGravity.center;

        [JsonProperty("margins")]
        public Edges Margins { get; set; }
    }

    public class CellDivider
    {
        [JsonProperty("style")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CellDividerStyle Style { get; set; } = CellDividerStyle.@default;

        [JsonProperty("size")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CellDividerSize Size { get; set; } = CellDividerSize.d1;
    }

    public class ButtonCell : SCell
    {
        public override string Type { get; } = "button_cell_view";
        
        [JsonProperty("paddings")]
        public Edges Paddings { get; set; }

        public Button Content { get; }

        public ButtonCell(string text, params SAction[] actions)
        {
            Content = new Button(text, actions);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CellDividerStyle
    {
        @default,
        read_only
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CellDividerSize
    {
        d1,
        d2,
        d3,
        d4,
        d5
    }
}