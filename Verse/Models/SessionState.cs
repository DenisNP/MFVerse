using System.Collections.Generic;

namespace Verse.Models
{
    public class SessionState
    {
        public Syllable[][] Syllables { get; set; }
        public FootType CurrentFoot { get; set; }
    }
}