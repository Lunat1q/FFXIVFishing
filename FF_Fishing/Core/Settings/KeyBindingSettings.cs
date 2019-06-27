using System.ComponentModel;
using WindowsInput.Native;
using Newtonsoft.Json;

namespace FF_Fishing.Core.Settings
{
    public class KeyBindingSettings
    {
        [DisplayName("Cast key")]
        [DefaultValue(VirtualKeyCode.VK_2)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode CastKey { get; set; } = VirtualKeyCode.VK_2;

        [DisplayName("Hook key")]
        [DefaultValue(VirtualKeyCode.VK_3)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode HookKey { get; set; } = VirtualKeyCode.VK_3;

        [DisplayName("Release key")]
        [DefaultValue(VirtualKeyCode.VK_4)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode ReleaseKey { get; set; } = VirtualKeyCode.VK_4;

        [DisplayName("Mooch key")]
        [DefaultValue(VirtualKeyCode.VK_5)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode MoochKey { get; set; } = VirtualKeyCode.VK_5;

        [DisplayName("Quit key")]
        [DefaultValue(VirtualKeyCode.VK_6)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode QuitKey { get; set; } = VirtualKeyCode.VK_6;
    }
}
