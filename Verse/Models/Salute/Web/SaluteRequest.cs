using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Verse.Models.Salute.Web
{
    public class SaluteRequest
    {
        [JsonProperty("messageName")]
        public string MessageName { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("messageId")]
        public long MessageId { get; set; }

        [JsonProperty("uuid")]
        public Uuid Uuid { get; set; }

        [JsonProperty("payload")]
        public SaluteRequestPayload Payload { get; set; }

        [JsonIgnore]
        public string UserId => string.IsNullOrEmpty(Uuid.Sub) ? Uuid.UserId : Uuid.Sub;

        [JsonIgnore]
        public string[] Tokens => Payload?.Message?.Tokens ?? Array.Empty<string>();

        [JsonIgnore]
        public int[] Numbers => Tokens
            .Select(t => int.TryParse(t, out int n) ? (int?) n : null)
            .Where(n => n.HasValue)
            .Select(n => n.Value)
            .ToArray();
        
        [JsonIgnore]
        public string[] Lemmas => Payload?.Message?.Lemmas ?? Array.Empty<string>();

        [JsonIgnore]
        public bool IsEnter => Payload.NewSession || MessageName == "RUN_APP";
    }

    public class Uuid
    {
        [JsonProperty("userChannel")]
        public string UserChannel { get; set; }

        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
    
    public class SaluteRequestPayload
    {
        [JsonProperty("device")]
        public Device Device { get; set; }

        [JsonProperty("app_info")]
        public AppInfo AppInfo { get; set; }

        [JsonProperty("character")]
        public Character Character { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("strategies")]
        public Strategies Strategies { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("new_session")]
        public bool NewSession { get; set; }

        [JsonProperty("server_action")]
        public ServerAction ServerAction { get; set; }
    }

    public class ServerAction
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("payload")]
        public Dictionary<string, object> Payload { get; set; } = new();
    }

    public class AppInfo
    {
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }

        [JsonProperty("appversionId")]
        public string AppversionId { get; set; }
    }

    public class Character
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CharacterId Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("gender")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CharacterGender Gender { get; set; }

        [JsonProperty("appeal")]
        public string Appeal { get; set; }
    }

    public enum CharacterId
    {
        Sber,
        Athena,
        Joy
    }

    public enum CharacterGender
    {
        Male,
        Female
    }

    public class Device
    {
        [JsonProperty("platformType")]
        public string PlatformType { get; set; }

        [JsonProperty("platformVersion")]
        public string PlatformVersion { get; set; }

        [JsonProperty("surface")]
        public string Surface { get; set; }

        [JsonProperty("surfaceVersion")]
        public string SurfaceVersion { get; set; }

        [JsonProperty("features")]
        public Features Features { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
        
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("deviceManufacturer")]
        public string DeviceManufacturer { get; set; }

        [JsonProperty("deviceModel")]
        public string DeviceModel { get; set; }
    }

    public class Capabilities
    {
        [JsonProperty("screen")]
        public Availability Screen { get; set; }

        [JsonProperty("mic")]
        public Availability Mic { get; set; }

        [JsonProperty("speak")]
        public Availability Speak { get; set; }
    }

    public class Availability
    {
        [JsonProperty("available")]
        public bool Available { get; set; }
    }

    public class Features
    {
        [JsonProperty("appTypes")]
        public string[] AppTypes { get; set; }
    }

    public class Message
    {
        private readonly Regex _cyrillic = new(@"^[а-яё0-9\-]+$");
        private readonly Regex _latin = new(@"^[a-z0-9\-\`\']+$");
        
        [JsonProperty("original_text")]
        public string OriginalText { get; set; }

        [JsonProperty("normalized_text")]
        public string NormalizedText { get; set; }

        [JsonProperty("asr_normalized_message")]
        public string AsrNormalizedMessage { get; set; }

        [JsonProperty("tokenized_elements_list")]
        public TokenizedElement[] TokenizedElementsList { get; set; }

        [JsonIgnore]
        public string[] Tokens => TokenizedElementsList
            .Select(t => t.RawText.ToLower().Trim())
            .Where(t => t.Length > 0 && (_cyrillic.IsMatch(t) || _latin.IsMatch(t)))
            .ToArray();
        
        [JsonIgnore]
        public string[] Lemmas => TokenizedElementsList
            .Select(t => t.Lemma.ToLower().Trim())
            .Where(t => t.Length > 0 && (_cyrillic.IsMatch(t) || _latin.IsMatch(t)))
            .ToArray();
    }

    public class TokenizedElement
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        
        [JsonProperty("raw_text")]
        public string RawText { get; set; }
        
        [JsonProperty("lemma")]
        public string Lemma { get; set; }
    }

    public class Meta
    {
        [JsonProperty("time")]
        public Time Time { get; set; }
    }

    public class Time
    {
        [JsonProperty("timezone_id")]
        public string TimezoneId { get; set; }

        [JsonProperty("timezone_offset_sec")]
        public long TimezoneOffsetSec { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }

    public class Strategies
    {
        [JsonProperty("happy_birthday")]
        public bool HappyBirthday { get; set; }

        [JsonProperty("last_call")]
        public long? LastCall { get; set; }
    }
}
