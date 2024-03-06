using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AdCapGame
{
    public partial class PrestigeMenu : Window
    {
        private IBusinessManager businessManager;

        private static PrestigeMenu _instance;

        public static PrestigeMenu GetInstance(IBusinessManager manager)
        {
            if (_instance == null || !_instance.IsLoaded)
            {
                _instance = new PrestigeMenu(manager);
            }
            return _instance;
        }

        public PrestigeMenu(IBusinessManager manager)
        {
            InitializeComponent();
            businessManager = manager;
            PopulatePrestigeDetails();
        }

        private void PopulatePrestigeDetails()
        {
            var newPrestigeLevels = PrestigeManager.CalculateNewPrestigeLevels(); // Assuming this method exists and calculates correctly
            var prestigeLevelsInfo = $"Current Prestige Levels: {PlayerValues.PrestigeLevels} ({(PlayerValues.PrestigeLevels * 2)}%), New Levels on Reset: {newPrestigeLevels}";
            var totalEarningsInfo = $"Total Lifetime Earnings: {PlayerValues.LifetimeEarnings.ToKMBTQ()}";
            var benefitsInfo = "Each Prestige Level increases all profits by 2%.";

            PrestigeListPanel.Children.Add(CreateTextBlock(prestigeLevelsInfo, 0));
            PrestigeListPanel.Children.Add(CreateTextBlock(totalEarningsInfo, 0));
            PrestigeListPanel.Children.Add(CreateTextBlock(benefitsInfo, 0));

            // Only add the reset button if new Prestige Levels can be gained
            if (newPrestigeLevels > 0)
            {
                PrestigeListPanel.Children.Add(CreateResetButton());
            }
            else
            {
                PrestigeListPanel.Children.Add(CreateTextBlock("No new Prestige Levels to gain from reset.", 0));
            }
        }

        private TextBlock CreateTextBlock(string text, int column)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10),
                Foreground = Brushes.White
            };
            return textBlock;
        }

        private Button CreateResetButton()
        {
            var button = new Button
            {
                Content = "Reset Game & Claim Prestige Levels",
                Margin = new Thickness(10),
                Background = Brushes.DarkRed,
                Foreground = Brushes.White,
                Padding = new Thickness(10)
            };
            button.Click += ResetButton_Click;
            return button;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            PrestigeManager.ResetGame(); // Update this with actual reset logic that calculates and applies new Prestige Levels
            MessageBox.Show("Game reset! New Prestige Levels have been claimed.", "Reset Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}
