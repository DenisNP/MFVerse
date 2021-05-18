using Newtonsoft.Json;

namespace Verse.Models.Salute.Simple
{
    public class Edges
    {
        [JsonProperty("left")]
        public string Left { get; set; }
        
        [JsonProperty("right")]
        public string Right { get; set; }
        
        [JsonProperty("top")]
        public string Top { get; set; }
        
        [JsonProperty("bottom")]
        public string Bottom { get; set; }

        public Edges(Edge left, Edge right, Edge top, Edge bottom)
        {
            Left = Convert(left);
            Right = Convert(right);
            Top = Convert(top);
            Bottom = Convert(bottom);
        }

        public static string Convert(Edge value)
        {
            return $"{(int)value}x";
        }
        
        public static Edges All6()
        {
            return new(Edge.X6, Edge.X6, Edge.X6, Edge.X6);
        }
    }
    
    public enum Edge
    {
        X0 = 0,
        X1 = 1,
        X2 = 2,
        X4 = 4,
        X5 = 5,
        X6 = 6,
        X8 = 8,
        X9 = 9,
        X10 = 10,
        X12 = 12,
        X16 = 16
    }
}