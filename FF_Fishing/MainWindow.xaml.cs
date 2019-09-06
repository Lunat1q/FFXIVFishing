using System;
using System.Windows;
using System.Windows.Controls;
using FF_Fishing.Controller;
using FF_Fishing.Core.Settings;
using FF_Fishing.Pages;
using FF_Fishing.Pages.UIBuilders;
using FF_Fishing.Pages.UIBuilders.Proxy;

namespace FF_Fishing
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainFishingLogic _logic;
        public MainWindow()
        {
            _logic = new MainFishingLogic(FishingSettings.Settings, Dispatcher);
            DataContext = _logic;
            _logic.NewLogEntry += LogicOnNewLogEntry;
            InitializeComponent();
            InitDynamicElements();
        }

        private void InitDynamicElements()
        {
            SettingsUGrid.Children.Add(
                SettingsAutoUI.CreateSettingsClassController(
                    FishingSettings.Settings,
                    "Settings"
                )
            );
        }

        private void LogicOnNewLogEntry(object sender, EventArgs e)
        {
            LogBox.Items.MoveCurrentToLast();
            LogBox.ScrollIntoView(LogBox.Items.CurrentItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            if (_logic.Running)
            {
                btn.Content = "Start";
                _logic.StopFishing();
            }
            else
            {
                btn.Content = "Stop";
                _logic.StartFishing();
            }
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FishingSettings.Settings.Save();
        }
    }
}
