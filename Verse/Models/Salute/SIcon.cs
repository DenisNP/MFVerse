using Newtonsoft.Json;
using Verse.Models.Salute.Simple;

namespace Verse.Models.Salute
{
    public class SIcon
    {
        [JsonProperty("address")]
        public IconAddress Address { get; set; }

        [JsonProperty("size")]
        public Size Size { get; }

        [JsonProperty("margins")]
        public Edges Margins { get; set; }

        public SIcon(string url, string hash, SizeType size)
        {
            Address = new IconAddress(url, hash);
            Size = new Size
            {
                Width = size,
                Height = size
            };
        }
    }

    public class IconAddress
    {
        [JsonProperty("type")]
        public string Type { get; } = "url";

        [JsonProperty("url")]
        public string Url { get; }
        
        [JsonProperty("hash")]
        public string Hash { get; }

        public IconAddress(string url, string hash)
        {
            Url = url;
            Hash = hash;
        }
    }
}