using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Ring.ViewModels.MainScreen;

namespace Ring.Views.MainScreen
{
    public partial class StorageTankGroup : UserControl
    {
        public StorageTankGroup()
        {
            InitializeComponent();
            DataContext = new StorageTankGroupViewModel();
            PopulateComponentGrid1();
            PopulateComponentGrid2();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as Ring.MainWindow;
            if (mainWindow != null)
            {
                var mainContentArea = mainWindow.FindName("MainContentArea") as ContentControl;
                if (mainContentArea != null)
                {
                    mainContentArea.Content = new Ring.Views.Dashboard.DashboardView();
                }
            }
        }

        /// <summary>
        /// Shows the component details popup for Storage Tank 1
        /// </summary>
        private void ShowComponentsButton1_Click(object sender, RoutedEventArgs e)
        {
            ComponentPopup1.IsOpen = true;
        }

        /// <summary>
        /// Shows the component details popup for Storage Tank 2
        /// </summary>
        private void ShowComponentsButton2_Click(object sender, RoutedEventArgs e)
        {
            ComponentPopup2.IsOpen = true;
        }

        /// <summary>
        /// Populates the component grid for Storage Tank 1
        /// </summary>
        private void PopulateComponentGrid1()
        {
            // Component data: Name, Status (true = On/Open/High, false = Off/Closed/Low)
            var components = new[]
            {
                ("Agitator motor", true),
                ("Water pump", false),
                ("Level transmitter", true),
                ("Temperature probe", true),
                ("Inlet solenoid valve", false)
            };

            for (int i = 0; i < components.Length; i++)
            {
                var (componentName, isOn) = components[i];
                var row = i / 2;  // 2 columns per row
                var col = i % 2;

                // Create component container
                var border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(4),
                    Padding = new Thickness(10, 8, 10, 8),
                    ToolTip = $"{componentName} - {(isOn ? "On" : "Off")}"
                };

                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

                // Create grid for label and status light alignment
                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Component name (left-aligned)
                var textBlock = new TextBlock
                {
                    Text = componentName,
                    FontSize = 13,
                    FontWeight = FontWeights.Normal,
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    TextWrapping = TextWrapping.NoWrap
                };
                System.Windows.Controls.Grid.SetColumn(textBlock, 0);

                // Status indicator (right-aligned)
                var statusLight = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = isOn 
                        ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green
                        : new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                System.Windows.Controls.Grid.SetColumn(statusLight, 1);

                innerGrid.Children.Add(textBlock);
                innerGrid.Children.Add(statusLight);
                border.Child = innerGrid;

                ComponentGrid1.Children.Add(border);
            }
        }

        /// <summary>
        /// Populates the component grid for Storage Tank 2
        /// </summary>
        private void PopulateComponentGrid2()
        {
            // Component data: Name, Status (true = On/Open/High, false = Off/Closed/Low)
            var components = new[]
            {
                ("Agitator motor", false),
                ("Water pump", true),
                ("Level transmitter", true),
                ("Temperature probe", false),
                ("Inlet solenoid valve", true)
            };

            for (int i = 0; i < components.Length; i++)
            {
                var (componentName, isOn) = components[i];
                var row = i / 2;  // 2 columns per row
                var col = i % 2;

                // Create component container
                var border = new Border
                {
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    Background = Brushes.White,
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(4),
                    Padding = new Thickness(10, 8, 10, 8),
                    ToolTip = $"{componentName} - {(isOn ? "On" : "Off")}"
                };

                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

                // Create grid for label and status light alignment
                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // Component name (left-aligned)
                var textBlock = new TextBlock
                {
                    Text = componentName,
                    FontSize = 13,
                    FontWeight = FontWeights.Normal,
                    Foreground = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    TextWrapping = TextWrapping.NoWrap
                };
                System.Windows.Controls.Grid.SetColumn(textBlock, 0);

                // Status indicator (right-aligned)
                var statusLight = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = isOn 
                        ? new SolidColorBrush(Color.FromRgb(76, 175, 80))  // Green
                        : new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                System.Windows.Controls.Grid.SetColumn(statusLight, 1);

                innerGrid.Children.Add(textBlock);
                innerGrid.Children.Add(statusLight);
                border.Child = innerGrid;

                ComponentGrid2.Children.Add(border);
            }
        }
    }
}