using System.Linq;
using Verse.Models;
using Verse.Models.Salute;
using Verse.Models.Salute.Web;

namespace Verse.Helpers
{
    public static class SaluteExtensions
    {
        public const int DefaultMaxExcessAllowed = 3;

        public static bool HasWords(this SaluteRequest request, params string[] expected)
        {
            return HasWords(request, DefaultMaxExcessAllowed, expected);
        }
        
        public static bool HasWords(this SaluteRequest request, int maxExcessAllowed, params string[] expected)
        {
            return expected.Any(e => HasWords(request, e, maxExcessAllowed));
        }

        private static bool HasWords(SaluteRequest request, string expected, int maxExcessAllowed)
        {
            string[] words = expected.Split(" ");
            return Utils.IsSimilarTokens(words, request.Lemmas, maxExcessAllowed);
        }

        public static SaluteResponse AppendSendData(this SaluteResponse response, params object[] data)
        {
            response.Payload.Items.Add(new CommandItem(new SmartAppDataCommand(data)));
            return response;
        }

        public static SaluteResponse AppendText(this SaluteResponse response, string text, bool showBubble = false)
        {
            response.Payload.PronounceText = text;
            if (showBubble)
                response.Payload.Items.Add(new BubbleItem(text));
            
            return response;
        }

        public static SaluteResponse AppendText(
            this SaluteResponse response,
            SaluteRequest request,
            Phrase phrase,
            bool showBubble = false
        )
        {
            return AppendText(response, phrase.For(request), showBubble);
        }

        public static SaluteResponse AppendSuggestions(this SaluteResponse response, params string[] suggestions)
        {
            response.Payload.Suggestions = new Suggestions(suggestions);
            return response;
        }
    }
}