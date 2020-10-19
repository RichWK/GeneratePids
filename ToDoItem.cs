using Newtonsoft.Json;

namespace REBGV.Functions
{
    public class ToDoItem
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        public string Description { get; set; }
    }
}

