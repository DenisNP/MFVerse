using System;
using Newtonsoft.Json;

namespace Verse.Models
{
    public record UserState
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("last_enter")]
        public DateTime LastEnter { get; set; } = DateTime.UnixEpoch;
    }
}