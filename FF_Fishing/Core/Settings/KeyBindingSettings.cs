using System.ComponentModel;
using WindowsInput.Native;
using Newtonsoft.Json;
using TiqUtils.Wpf.UIBuilders;

namespace FF_Fishing.Core.Settings
{
    [DisplayName("Binding settings")]
    public class KeyBindingSettings
    {
        [DisplayName("Cast key")]
        [DefaultValue(VirtualKeyCode.VK_2)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode CastKey { get; set; } = VirtualKeyCode.VK_2;

        [PropertyMember]
        [DefaultValue(VirtualKeyCode.VK_3)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode HookKey { get; set; } = VirtualKeyCode.VK_3;

        [PropertyMember]
        [DefaultValue(VirtualKeyCode.VK_4)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode ReleaseKey { get; set; } = VirtualKeyCode.VK_4;

        [PropertyMember]
        [DefaultValue(VirtualKeyCode.VK_5)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode MoochKey { get; set; } = VirtualKeyCode.VK_5;

        [PropertyMember]
        [DefaultValue(VirtualKeyCode.VK_6)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode QuitKey { get; set; } = VirtualKeyCode.VK_6;

        [PropertyMember]
        [DefaultValue(VirtualKeyCode.VK_E)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public VirtualKeyCode PatienceKey { get; set; } = VirtualKeyCode.VK_E;
    }
}
