using Newtonsoft.Json;

namespace REBGV.Functions
{
    public class PidDbItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("currentPid")]
        public string CurrentPid { get; set; }
    }
}

