using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using TiqUtils.SettingsController;
using TiqUtils.Wpf.UIBuilders;

namespace FF_Fishing.Core.Settings
{
    [DisplayName("Main fishing settings")]
    public class FishingSettings
    {
        private static readonly SettingsController<FishingSettings> SettingsController;
        private KeyBindingSettings _bindingSettings;
        private TimersSettings _timersSettings;
        internal static FishingSettings Settings => SettingsController.Settings;

        static FishingSettings()
        {
            SettingsController = new SettingsController<FishingSettings>(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        [PropertyMember]
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UseLure { get; set; }

        [PropertyMember]
        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool ReleaseFish { get; set; }

        [PropertyMember]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool WindowTopMost { get; set; } = true;

        [PropertyMember]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Mooch { get; set; } = true;

        [PropertyMember]
        [DefaultValue(3)]
        [SliderLimits(1, 10)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public double BiteSoundMultiplier { get; set; } = 3;

        [PropertyMember]
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool UsePatience { get; set; } = true;

        [PropertyMember]
        [JsonProperty]
        public KeyBindingSettings BindingSettings
        {
            get => _bindingSettings ?? (_bindingSettings = new KeyBindingSettings());
            set => _bindingSettings = value;
        }

        [PropertyMember]
        [JsonProperty]
        public TimersSettings TimersSettings
        {
            get => _timersSettings ?? (_timersSettings = new TimersSettings());
            set => _timersSettings = value;
        }


        public void Save()
        {
            if (Settings == this)
            {
                SettingsController.Save();
            }
            else
            {
                throw new ArgumentException(nameof(FishingSettings));
            }
        }
    }
}