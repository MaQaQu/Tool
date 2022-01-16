using Newtonsoft.Json;

namespace YouiToolkit.Models
{
    [JsonObject]
    internal class AvoidObstacleShortCutText
    {
        [JsonProperty("i")]
        public int Index { get; set; } = -1;

        [JsonProperty("p")]
        public object[][] DragPoints { get; set; } = null;
    }
}
