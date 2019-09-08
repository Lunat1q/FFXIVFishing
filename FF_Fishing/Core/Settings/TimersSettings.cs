using System.ComponentModel;
using Newtonsoft.Json;
using TiqUtils.Wpf.UIBuilders;

namespace FF_Fishing.Core.Settings
{
    [DisplayName("Timer settings")]
    public class TimersSettings
    {
        [PropertyMember]
        [DefaultValue(4000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int AfterCast { get; set; } = 4000;

        [PropertyMember]
        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int BeforeCast { get; set; } = 3000;

        [PropertyMember]
        [DefaultValue(20000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int HookWait { get; set; } = 20000;

        [PropertyMember]
        [DefaultValue(12000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int TackleHook { get; set; } = 12000;

        [PropertyMember]
        [DefaultValue(6000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int BaitHook { get; set; } = 6000;

        [PropertyMember]
        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Gather { get; set; } = 3000;

        [PropertyMember]
        [DefaultValue(3000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int FailRetry { get; set; } = 3000;

        [PropertyMember]
        [DefaultValue(4000)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int Finish { get; set; } = 4000;

    }
}