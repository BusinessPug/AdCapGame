using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace AdCapGame
{
    public partial class BusinessUnlocks : Window
    {
        private static BusinessUnlocks _instance;

        private const string JsonFolderPath = "Business-Unlocks"; // Adjust this path to where your JSON files are stored

        public static HashSet<string> unlockedItems = new HashSet<string>();
        private const int Columns = 8; // Number of columns in the grid

        public static BusinessUnlocks GetInstance()
        {
            if (_instance == null || !_instance.IsLoaded)
            {
                _instance = new BusinessUnlocks();
            }
            return _instance;
        }

        private BusinessUnlocks()
        {
            InitializeComponent();
            MakeTheCollapsedGrids();
        }

        private void MakeTheCollapsedGrids()
        {
            string[] businessNames = { "Peak Insights", "Data Heights", "River Metrics", "Tree Path Lab", "Moon Tech Co", "Sky Growth", "Hill Trends", "Star Analysis", "Cloud Market", "Wave Stats", "All Businesses" };

            foreach (var businessName in businessNames)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5),
                    Tag = businessName.Replace(" ", "") + "Header"
                };

                var textBlock = new TextBlock
                {
                    Text = businessName,
                    FontSize = 20,
                    Foreground = System.Windows.Media.Brushes.White,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var button = new Button
                {
                    Content = "Expand",
                    Style = FindResource("ButtonStyle") as Style,
                    FontSize = 20,
                    Background = System.Windows.Media.Brushes.Black,
                    Foreground = System.Windows.Media.Brushes.White,
                    Margin = new Thickness(5)
                };

                button.Click += (sender, e) => ToggleGridVisibility(stackPanel, businessName, sender as Button);

                stackPanel.Children.Add(textBlock);
                stackPanel.Children.Add(button);

                BusinessUnlocksPanel.Children.Add(stackPanel);
            }
        }

        private void ToggleGridVisibility(StackPanel parentStackPanel, string businessName, Button button)
        {
            var index = BusinessUnlocksPanel.Children.IndexOf(parentStackPanel);
            if (button.Content.ToString() == "Expand")
            {
                var unlocksGrid = LoadBusinessUnlocksGrid(businessName);
                if (index != -1)
                {
                    BusinessUnlocksPanel.Children.Insert(index + 1, unlocksGrid);
                }
                button.Content = "Collapse";
            }
            else
            {
                if (index != -1 && index + 1 < BusinessUnlocksPanel.Children.Count)
                {
                    BusinessUnlocksPanel.Children.RemoveAt(index + 1);
                }
                button.Content = "Expand";
            }
        }

        private Grid LoadBusinessUnlocksGrid(string businessName)
        {
            string jsonFileName = $"{businessName.Replace(" ", "-")}-Unlocks.json";
            string jsonFilePath = Path.Combine(JsonFolderPath, jsonFileName);
            string json = File.ReadAllText(jsonFilePath);
            var unlocks = JsonConvert.DeserializeObject<List<UnlockItem>>(json);

            var unlocksGrid = new Grid
            {
                Margin = new Thickness(5, 0, 5, 5)
            };

            for (int i = 0; i < Columns; i++)
            {
                unlocksGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int row = 0, column = 0;
            foreach (var unlock in unlocks)
            {
                if (column >= Columns)
                {
                    column = 0;
                    row++;
                }
                string imagePath = GetImagePath(businessName);

                var image = new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Relative)),
                    Tag = (unlock.UnlockAmount.ToString() + " Owned: " + unlock.Description), // Store description in Tag for access on hover
                    Opacity = unlockedItems.Contains(businessName + " " + unlock.UnlockAmount) ? 1.0 : 0.5,
                    Width = 80, // Set appropriate size
                    Height = 80,
                    Margin = new Thickness(5)
                };

                image.MouseEnter += (s, e) => ShowDescription(s as Image);
                image.MouseLeave += (s, e) => HideDescription();

                if (unlocksGrid.RowDefinitions.Count <= row)
                {
                    unlocksGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }

                Grid.SetRow(image, row);
                Grid.SetColumn(image, column);

                unlocksGrid.Children.Add(image);

                column++;
            }

            return unlocksGrid;
        }

        private string GetImagePath(string businessName)
        {
            switch (businessName)
            {
                case "Peak Insights":
                    return "Assets/Peak-Insights.png";
                case "Data Heights":
                    return "Assets/Data-Heights.png";
                case "River Metrics":
                    return "Assets/River-Metrics.png";
                case "Tree Path Lab":
                    return "Assets/Tree-Path-Lab.png";
                case "Moon Tech Co":
                    return "Assets/Moon-Tech-Co.png";
                case "Sky Growth":
                    return "Assets/Sky-Growth.png";
                case "Hill Trends":
                    return "Assets/Hill-Trends.png";
                case "Star Analysis":
                    return "Assets/Star-Analysis.png";
                case "Cloud Market":
                    return "Assets/Cloud-Market.png";
                case "Wave Stats":
                    return "Assets/Wave-Stats.png";
                case "All Businesses":
                    return "Assets/All-Businesses.png";
                default:
                    return "Assets/AdCapIcon.ico";
            }
        }

        private void ShowDescription(Image image)
        {
            var description = image.Tag as string;
            // Display description, e.g., in a tooltip, status bar, or another designated area
            image.ToolTip = description;
        }

        private void HideDescription()
        {
            // Hide the description when the mouse leaves
            // Example: image.ToolTip = null;
        }

        public class UnlockItem
        {
            public int UnlockAmount { get; set; }
            public string Description { get; set; }
        }
    }
}
