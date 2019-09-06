using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using FF_Fishing.Pages.UIBuilders.Proxy;
using TiQWpfUtils.Converters;
using TiQWpfUtils.Helpers;

namespace FF_Fishing.Pages.UIBuilders
{
    // ReSharper disable once InconsistentNaming
    public class SettingsAutoUI : Window
    {
        private readonly object _settings;
        private Grid _settingsGrid;

        public SettingsAutoUI(object settingsClass)
        {
            _settings = settingsClass;
            InitializeComponent();
            GenerateBindingControllers();
            DataContext = settingsClass;
        }

        private void InitializeComponent()
        {
            InitWindowSettings();

            var baseGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            baseGrid.RowDefinitions.Add(new RowDefinition());
            baseGrid.RowDefinitions.Add(new RowDefinition());

            _settingsGrid = new Grid();
            baseGrid.Children.Add(_settingsGrid);

            var closeButton = new Button
            {
                Content = "Close"
            };
            closeButton.Click += CloseButtonOnClick;
            Grid.SetRow(closeButton, 1);

            baseGrid.Children.Add(closeButton);

            this.Content = baseGrid;
        }

        private void CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Type GetObjectType()
        {
            if (_settings is NotifyPropertyChangedProxy proxy)
            {
                return proxy.GetWrappedType();
            }

            return _settings.GetType();
        }

        private void GenerateBindingControllers()
        {
            var properties = this.GetObjectType().GetProperties();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var rowIdx = 0;
            foreach (var prop in properties.Where(x => x.GetCustomAttribute<DataMemberAttribute>() != null))
            {
                grid.RowDefinitions.Add(new RowDefinition());
                var nameAttribute = prop.GetCustomAttribute<DisplayNameAttribute>();
                var name = nameAttribute?.DisplayName ?? GetBeautyPropName(prop);
                var text = new TextBlock { Text = name, Margin = new Thickness(5) };
                grid.Children.Add(text);
                Grid.SetColumn(text, 0);
                Grid.SetRow(text, rowIdx);

                UIElement e;

                switch (prop.PropertyType)
                {
                    case Type _ when prop.PropertyType == typeof(bool):
                        e = CreateBooleanController(prop);
                        break;
                    case Type _ when prop.PropertyType.IsEnum:
                        e = CreateEnumController(prop);
                        break;
                    case Type _ when prop.PropertyType.IsClass:
                        e = CreateSettingsClassController(GetPropertyValue(prop));
                        break;
                    case Type _ when prop.PropertyType == typeof(double):
                        e = CreateDoubleController(prop);
                        break;
                    case Type _ when prop.PropertyType == typeof(int):
                        e = CreateIntController(prop);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Grid.SetColumn(e, 1);
                Grid.SetRow(e, rowIdx);
                grid.Children.Add(e);
                rowIdx++;
            }

            _settingsGrid.Children.Add(grid);
        }

        private object GetPropertyValue(PropertyInfo prop)
        {
            if (_settings is NotifyPropertyChangedProxy proxy)
            {
                return prop.GetValue(proxy.WrappedObject);
            }
            return prop.GetValue(_settings);
        }

        private static string GetBeautyPropName(PropertyInfo prop)
        {
            var name = prop.Name;
            var sb = new StringBuilder();
            
            foreach (var c in name)
            {
                var curChar = c;
                if (char.IsUpper(curChar))
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(' ');
                        curChar = char.ToLower(c);
                    }
                }
                sb.Append(curChar);
            }
            return sb.ToString();
        }

        private void InitWindowSettings()
        {
            var type = this.GetObjectType();
            var formName = type.GetCustomAttribute<DisplayNameAttribute>();
            this.Title = formName?.DisplayName ?? type.Name;
            this.MinWidth = 300;
            this.MinHeight = 100;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        internal static Button CreateSettingsClassController(object obj, string buttonText = "Open")
        {
            var button = new Button
            {
                Content = buttonText,
                Margin = new Thickness(5)
            };

            button.Click += (sender, args) =>
            {
                if (!(obj is INotifyPropertyChanged))
                {
                    obj = new NotifyPropertyChangedProxy(obj);
                }
                var settingsPage = new SettingsAutoUI(obj)
                {
                    Topmost = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                settingsPage.ShowDialog();

            };

            return button;
        }

        private static ComboBox CreateEnumController(PropertyInfo prop)
        {
            var cb = new ComboBox
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                DisplayMemberPath = nameof(ValueDescription.Description),
                SelectedValuePath = nameof(ValueDescription.Value)
            };
            var collectionBinding = new Binding
            {
                Path = new PropertyPath(prop.Name),
                Converter = EnumToCollectionConverter.Instance
            };
            cb.SetBinding(ItemsControl.ItemsSourceProperty, collectionBinding);
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name),
                Converter = EnumValueConverter.Instance
            };
            cb.SetBinding(Selector.SelectedItemProperty, valueBinding);
            return cb;
        }

        private static CheckBox CreateBooleanController(PropertyInfo prop)
        {
            var cb = new CheckBox
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name)
            };
            cb.SetBinding(ToggleButton.IsCheckedProperty, valueBinding);
            return cb;
        }

        private static Slider CreateDoubleController(PropertyInfo prop)
        {
            var limits = prop.GetCustomAttribute<SliderLimitsAttribute>();
            var ret = new Slider
            {
                AutoToolTipPlacement = AutoToolTipPlacement.TopLeft,
                Maximum = limits?.Max ?? 10,
                Minimum = limits?.Min ?? 1,
                TickFrequency = 0.05,
                AutoToolTipPrecision = 2,
                VerticalAlignment = VerticalAlignment.Center
            };
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name)
            };
            ret.SetBinding(RangeBase.ValueProperty, valueBinding);
            return ret;
        }

        private static TextBox CreateIntController(PropertyInfo prop)
        {
            var ret = new TextBox
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            var valueBinding = new Binding
            {
                Path = new PropertyPath(prop.Name)
            };
            ret.SetBinding(TextBox.TextProperty, valueBinding);
            return ret;
        }
    }
}
