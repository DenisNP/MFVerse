using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Verse.Models.Salute.Simple
{
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; }
        
        [JsonProperty("hash")]
        public string Hash { get; }

        [JsonProperty("size")]
        public ImageSize Size { get; }

        [JsonProperty("scale_mode")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ScaleMode ScaleMode { get; set; } = ScaleMode.scale_aspect_fill;

        public Image(string url, string hash, ImageSize size)
        {
            Url = url;
            Hash = hash;
            Size = size;
        }
    }

    public class ImageSize
    {
        [JsonProperty("width")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SizeType Width { get; set; }

        [JsonProperty("aspect_ratio")]
        public double AspectRatio { get; }

        public ImageSize(double aspectRatio, SizeType width = SizeType.medium)
        {
            Width = width;
            AspectRatio = aspectRatio;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum ScaleMode
    {
        scale_aspect_fill,
        scale_aspect_fit,
        center,
        top,
        bottom,
        left,
        right,
        top_left,
        top_right,
        bottom_left,
        bottom_right
    }
}