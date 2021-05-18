using System;
using System.Linq;

namespace Verse.Models
{
    public class Foot
    {
        public FootType Type { get; }
        
        public string Name => Type switch {
            FootType.Unknown => "",
            FootType.Iambic => "ямб",
            FootType.Chorea => "хорей",
            FootType.Dactyl => "дактиль",
            FootType.Amphibrachium => "амфибрахий",
            FootType.Anapest => "анапест",
            _ => throw new ArgumentOutOfRangeException()
        };
        public string Description => Type switch {
            FootType.Unknown => "",
            FootType.Iambic => "Ямб",
            FootType.Chorea => "Хорей",
            FootType.Dactyl => "Дактиль",
            FootType.Amphibrachium => "Амфибрахий",
            FootType.Anapest => "Анапест",
            _ => throw new ArgumentOutOfRangeException()
        };
        public int[] Mask => Type switch {
            FootType.Unknown => new []{0},
            FootType.Iambic => new []{0,1},
            FootType.Chorea => new []{1,0},
            FootType.Dactyl => new []{1,0,0},
            FootType.Amphibrachium => new []{0,1,0},
            FootType.Anapest => new []{0,0,1},
            _ => throw new ArgumentOutOfRangeException()
        };

        public string Schema => Type switch
        {
            FootType.Unknown => "",
            FootType.Iambic => "_ | _ |",
            FootType.Chorea => "| _ | _",
            FootType.Dactyl => "| _ _ | _ _",
            FootType.Amphibrachium => "_ | _ _ | _",
            FootType.Anapest => "_ _ | _ _ |",
            _ => throw new ArgumentOutOfRangeException()
        };

        public int[] GetMaskOfLength(int l)
        {
            return Enumerable.Range(0, l).Select(x => Mask[x % Mask.Length]).ToArray();
        }

        public Foot(FootType type)
        {
            Type = type;
        }
    }

    public enum FootType
    {
        Unknown,
        Iambic,
        Chorea,
        Dactyl,
        Amphibrachium,
        Anapest
    }
}