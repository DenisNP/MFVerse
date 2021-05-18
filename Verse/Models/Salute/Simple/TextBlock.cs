using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Verse.Models.Salute.Simple
{
    public class TextBlock
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("typeface")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SaluteTextTypeface Typeface { get; set; } = SaluteTextTypeface.text1;

        [JsonProperty("text_color")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SaluteTextColor TextColor { get; set; } = SaluteTextColor.@default;

        [JsonProperty("max_lines")]
        public int MaxLines { get; set; } = 99;

        [JsonProperty("margins")]
        public Edges Margins { get; set; }

        public TextBlock(string text)
        {
            Text = text;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SaluteTextTypeface
    {
        headline1,
        headline2,
        headline3,
        title1,
        title2,
        body1,
        body2,
        body3,
        text1,
        paragraphText1,
        paragraphText2,
        footnote1,
        footnote2,
        button1,
        button2,
        caption
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum SaluteTextColor
    {
        @default,
        secondary,
        tertiary,
        inverse,
        brand,
        warning,
        critical,
        link
    }
}