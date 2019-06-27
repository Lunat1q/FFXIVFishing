using System.ComponentModel;
using Newtonsoft.Json;

namespace FF_Fishing.Core.Settings
{
    public class TimersSettings
    {
        [DefaultValue(4000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int AfterCast { get; set; } = 4000;

        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int BeforeCast { get; set; } = 3000;

        [DefaultValue(20000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int HookWait { get; set; } = 20000;

        [DefaultValue(12000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int TackleHook { get; set; } = 12000;

        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int BaitHook { get; set; } = 3000;

        [DefaultValue(2000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Gather { get; set; } = 2000;

        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int FailRetry { get; set; } = 3000;

        [DefaultValue(4000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Finish { get; set; } = 4000;

    }
}