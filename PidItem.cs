using Newtonsoft.Json;

namespace REBGV.Functions
{
    public class PidItem
    {
        [JsonProperty("id")]
        public string id { get; set; }

        public string Description { get; set; }
    }
}

