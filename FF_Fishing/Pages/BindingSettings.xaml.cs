using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using FF_Fishing.Core.Settings;
using TiQWpfUtils.Converters;
using TiQWpfUtils.Helpers;

namespace FF_Fishing.Pages
{
    /// <summary>
    /// Interaction logic for BindingSettings.xaml
    /// </summary>
    public partial class BindingSettings
    {
        private readonly KeyBindingSettings _bindingSettings;

        public BindingSettings(KeyBindingSettings bindingSettings)
        {
            _bindingSettings = bindingSettings;
            InitializeComponent();
            GenerateBindingControllers();
            DataContext = bindingSettings;
        }

        private void GenerateBindingControllers()
        {
            var properties = _bindingSettings.GetType().GetProperties();
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var rowIdx = 0;
            var collectionConverter = new EnumToCollectionConverter();
            var enumValueConverter = new EnumValueConverter();
            foreach (var prop in properties)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                var nameAttribute = prop.GetCustomAttribute<DisplayNameAttribute>();
                var name = nameAttribute?.DisplayName ?? prop.Name;
                var text = new TextBlock {Text = name, Margin = new Thickness(5)};
                grid.Children.Add(text);
                Grid.SetColumn(text, 0);
                Grid.SetRow(text, rowIdx);

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
                    Converter = collectionConverter
                };
                cb.SetBinding(ItemsControl.ItemsSourceProperty, collectionBinding);
                var valueBinding = new Binding
                {
                    Path = new PropertyPath(prop.Name),
                    Converter = enumValueConverter
                };
                cb.SetBinding(Selector.SelectedItemProperty, valueBinding);


                grid.Children.Add(cb);
                Grid.SetColumn(cb, 1);
                Grid.SetRow(cb, rowIdx);


                rowIdx++;
            }

            KeySettings.Children.Add(grid);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
