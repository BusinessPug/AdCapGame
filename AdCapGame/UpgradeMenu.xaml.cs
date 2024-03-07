using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdCapGame
{
    public class UpgradeDefinition
    {
        public string Description { get; set; }
        public string CostText { get; set; }
        public double CostValue { get; set; }
    }

    public partial class UpgradeMenu : Window
    {
        private static UpgradeMenu _instance;
        public IBusinessManager businessManager;
        public static HashSet<string> purchasedUpgrades = new HashSet<string>();
        public List<Upgrade> upgrades;
        private static int upgradeIdCounter = 1;

        public static UpgradeMenu GetInstance(IBusinessManager manager)
        {
            if (_instance == null || !_instance.IsLoaded)
            {
                _instance = new UpgradeMenu(manager);
            }
            return _instance;
        }

        private UpgradeMenu(IBusinessManager manager)
        {
            InitializeComponent();
            upgradeIdCounter = 1;
            businessManager = manager;
            PopulateUpgrades();
        }

        private void PopulateUpgrades()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "upgrades.json");
                string json = File.ReadAllText(filePath);
                var upgradeDefinitions = JsonConvert.DeserializeObject<List<UpgradeDefinition>>(json);

                // Dictionary to map "Business x" to actual business names.
                var businessNameMappings = new Dictionary<string, string>
                {
                    { "Business 1", "Peak Insights" },
                    { "Business 2", "Data Heights" },
                    { "Business 3", "River Metrics" },
                    { "Business 4", "Tree Path Lab" },
                    { "Business 5", "Moon Tech Co." },
                    { "Business 6", "Sky Growth" },
                    { "Business 7", "Hill Trends" },
                    { "Business 8", "Star Analyis" },
                    { "Business 9", "Cloud Market" },
                    { "Business 10", "Wave Stats" }
                };

                upgrades = upgradeDefinitions?.Select(def => new Upgrade
                {
                    Id = GenerateId(),
                    Description = def.Description, // Keep the original description for logic
                    CostText = def.CostText,
                    CostValue = def.CostValue,
                    Apply = manager => manager.ApplyUpgrade(def.Description)
                }).ToList();

                foreach (var upgrade in upgrades)
                {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                    grid.Children.Add(CreateTextBlock(upgrade.Id, 0));

                    // Replace "Business x" in the description with the actual name, if it exists.
                    string displayDescription = upgrade.Description;
                    foreach (var mapping in businessNameMappings)
                    {
                        displayDescription = Regex.Replace(displayDescription, Regex.Escape(mapping.Key), mapping.Value);
                    }
                    grid.Children.Add(CreateTextBlock(displayDescription, 1)); // Use modified description

                    grid.Children.Add(CreateTextBlock(upgrade.CostText, 2));
                    grid.Children.Add(CreateUpgradeButton(upgrade, 3));

                    UpgradeListPanel.Children.Add(grid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load upgrades: {ex.Message}");
            }
        }


        private string GenerateId()
        {
            string generatedId = $"Up{upgradeIdCounter}";
            upgradeIdCounter++; // Increment the counter for the next call
            return generatedId;
        }


        private TextBlock CreateTextBlock(string text, int column)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Foreground = Brushes.White,
                Padding = new Thickness(3)
            };
            Grid.SetColumn(textBlock, column); // Correctly set the column here
            return textBlock;
        }

        private Button CreateUpgradeButton(Upgrade upgrade, int column)
        {
            var isPurchased = purchasedUpgrades.Contains(upgrade.Id);
            var canAfford = PlayerValues.Money >= upgrade.CostValue;

            var button = new Button
            {
                Content = isPurchased ? "Purchased" : "Upgrade",
                Margin = new Thickness(5),
                Background = isPurchased ? Brushes.Gray : (canAfford ? Brushes.Green : Brushes.Red),
                IsEnabled = !isPurchased && canAfford,
                Style = (Style)FindResource("ButtonStyle"),
                Tag = upgrade // Store the entire Upgrade object
            };
            Grid.SetColumn(button, column);
            button.Click += UpgradeButton_Click;
            return button;
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var upgrade = (Upgrade)button.Tag; // Direct cast from Tag to Upgrade

            if (PlayerValues.Money >= upgrade.CostValue)
            {
                PlayerValues.Money -= upgrade.CostValue;
                upgrade.Apply(businessManager);
                purchasedUpgrades.Add(upgrade.Id); // Mark the upgrade as purchased.
                button.Content = "Purchased";
                button.Background = Brushes.Gray;
                button.IsEnabled = false;
                // Optionally, refresh UI here if needed to reflect new balance and upgrade status.
            }
        }
    }
}