using System;
using System.Linq;
using Newtonsoft.Json;
using Verse.Helpers;

namespace Verse.Models
{
    public class ParseResult
    {
        public Syllable[][] Syllables { get; set; } = null;
        public FootType FootType { get; set; } = FootType.Unknown;

        [JsonIgnore]
        public Foot Foot => new Foot(FootType);

        [JsonIgnore]
        public int StepsNum
        {
            get
            {
                if (Syllables == null || FootType == FootType.Unknown)
                    return 0;

                int syllablesCount = Syllables.Sum(w => w.Count(s => s.Type != SyllableType.Consonant));
                return (int)Math.Floor((double)syllablesCount / Foot.StepsCount);
            }
        }
        
        [JsonIgnore]
        public string StepsName => Utils.GetStepsName(StepsNum);

        [JsonIgnore]
        public string FullSchema => string.Join(" ", Enumerable.Repeat(Foot.Schema, StepsNum));

        public static ParseResult Empty => new ParseResult();
    }
}