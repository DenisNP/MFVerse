using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Verse.Models.Salute.Simple;

namespace Verse.Models.Salute.Web
{
    public class SaluteResponse
    {
        [JsonProperty("messageName")]
        public string MessageName { get; set; } = "ANSWER_TO_USER";

        [JsonProperty("sessionId")]
        public string SessionId { get; }

        [JsonProperty("messageId")]
        public long MessageId { get; }

        [JsonProperty("uuid")]
        public Uuid Uuid { get; }

        [JsonProperty("payload")]
        public SaluteResponsePayload Payload { get; set; }

        public SaluteResponse(SaluteRequest request) : this(request, new SaluteResponsePayload()) { }

        public SaluteResponse(SaluteRequest request, SaluteResponsePayload payload)
        {
            MessageId = request.MessageId;
            SessionId = request.SessionId;
            Uuid = request.Uuid;
            Payload = payload;
            payload.Device = request.Payload.Device;
        }

        public SaluteResponse(SaluteRequest request, string text, bool noBubble = false)
        {
            MessageId = request.MessageId;
            SessionId = request.SessionId;
            Uuid = request.Uuid;
            Payload = new SaluteResponsePayload
            {
                Device = request.Payload.Device,
                PronounceText = text,
            };

            if (!noBubble)
                Payload.Items.Add(new BubbleItem(text));
        }
    }
    
    public class SaluteResponsePayload
    {
        [JsonProperty("pronounceText")]
        public string PronounceText { get; set; }

        [JsonProperty("pronounceTextType")]
        public string PronounceTextType => !string.IsNullOrEmpty(PronounceText) && PronounceText.StartsWith("<speak>")
            ? "application/ssml"
            : "application/text";

        [JsonProperty("emotion")]
        public Emotion Emotion { get; set; }

        [JsonProperty("items")]
        public List<SItem> Items { get; init; } = new();

        [JsonProperty("suggestions")]
        public Suggestions Suggestions { get; set; }

        [JsonProperty("intent")]
        public string Intent { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("device")]
        public Device Device { get; set; }

        [JsonProperty("auto_listening")]
        public bool AutoListening { get; set; }

        [JsonProperty("asr_hints")]
        public AsrHints AsrHints { get; set; }

        [JsonProperty("finished")]
        public bool Finished { get; set; }
    }

    public class Suggestions
    {
        [JsonProperty("buttons")]
        public List<Suggestion> Buttons { get; }

        public Suggestions(params string[] suggestions)
        {
            Buttons = suggestions.Select(s => new Suggestion(s)).ToList();
        }
    }
    
    public class Emotion
    {
        [JsonProperty("emotionId")]
        public string EmotionId { get; set; }
    }

    public class AsrHints
    {
        [JsonProperty("words")]
        public string[] Words { get; set; }

        [JsonProperty("enable_letters ")]
        public bool EnableLetters { get; set; }

        [JsonProperty("nospeechtimeout")]
        public int? NoSpeechTimeout { get; set; }

        [JsonProperty("eou")]
        public int? EndOfUtterance { get; set; }
    }
}