using Newtonsoft.Json;

namespace REBGV.Functions
{
    public class PidItem
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("currentPid")]
        public string CurrentPid { get; set; }
    }
}

