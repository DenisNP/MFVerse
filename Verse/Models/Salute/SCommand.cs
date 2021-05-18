using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Verse.Models.Salute
{
    public abstract class SCommand
    {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }

    public class CloseAppCommand : SCommand
    {
        public override string Type => "close_app";
    }

    public class SmartAppDataCommand : SCommand
    {
        public override string Type => "smart_app_data";

        [JsonProperty("smart_app_data")]
        public Dictionary<string, object> Data { get; } = new();

        public SmartAppDataCommand(params object[] data)
        {
            if (data.Length % 2 != 0)
                throw new ArgumentException($"{nameof(data)} has wrong length");

            for (var i = 0; i < data.Length - 1; i += 2)
            {
                object key = data[i];
                
                if (key is string sKey)
                    Data.Add(sKey, data[i + 1]);
                else
                    throw new ArgumentException($"Wrong key type: {key}");
            }
        }
    }
}