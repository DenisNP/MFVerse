using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Verse.Models.Salute
{
    public class SmartAppCommand
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("smart_app_data")]
        public Dictionary<string, object> Data { get; set; }
    }

    public class SmartAppCommandConverter : JsonConverter<SmartAppCommand>
    {
        public override void WriteJson(JsonWriter writer, SmartAppCommand? value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
        }

        public override SmartAppCommand? ReadJson(
            JsonReader reader, Type objectType, SmartAppCommand? existingValue, bool hasExistingValue, JsonSerializer serializer
        )
        {
            try
            {
                JObject obj = JObject.Load(reader);
                string type = (string)obj["type"] ?? "";

                if (type == "smart_app_data")
                    return obj.ToObject<SmartAppCommand>();

                return new SmartAppCommand {Type = type};
            }
            catch (Exception e)
            {
                // ignored
            }
            
            return new SmartAppCommand();
        }
    }
}