using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace AdCapGame
{
    public partial class MainWindow : Window, IBusinessManager
    {
        private Business1 business1;
        private Business2 business2;
        private Business3 business3;
        private Business4 business4;
        private Business5 business5;
        private Business6 business6;
        private Business7 business7;
        private Business8 business8;
        private Business9 business9;
        private Business10 business10;
        private static List<Business> businesses = new List<Business>();
        private List<PopupWindow> openPopups = new List<PopupWindow>();
        private DispatcherTimer uiUpdateTimer;
        private DispatcherTimer upgradeTimer = new DispatcherTimer();
        private string currentButtonName = string.Empty;
        private Dictionary<string, Action> buttonToPurchaseActionMap;
        public List<Business> GetAllBusinesses() => new List<Business> { business1, business2, business3, business4, business5, business6, business7, business8, business9, business10 };

        public MainWindow()
        {
            InitializeComponent();
            PlayerValues.Money = 5e5;
            InitializeGame();

            buttonToPurchaseActionMap = new Dictionary<string, Action>
            {
                { "B1UpgradeButton", () => business1.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B2UpgradeButton", () => business2.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B3UpgradeButton", () => business3.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B4UpgradeButton", () => business4.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B5UpgradeButton", () => business5.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B6UpgradeButton", () => business6.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B7UpgradeButton", () => business7.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B8UpgradeButton", () => business8.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B9UpgradeButton", () => business9.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B10UpgradeButton", () => business10.Purchase().ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
            };

            // Initialize the timer
            upgradeTimer = new DispatcherTimer();
            upgradeTimer.Interval = TimeSpan.FromMilliseconds(50); // Adjust interval as needed
            upgradeTimer.Tick += UpgradeTimer_Tick;

            PrestigeManager.OnResetGame += ResetAndApplyPrestige;
            SetupAutoSaveTimer();
            this.Closing += MainWindow_Closing;
        }

        private async void InitializeGame()
        {
            business1 = new Business1(this);
            business2 = new Business2(this);
            business3 = new Business3(this);
            business4 = new Business4(this);
            business5 = new Business5(this);
            business6 = new Business6(this);
            business7 = new Business7(this);
            business8 = new Business8(this);
            business9 = new Business9(this);
            business10 = new Business10(this);

            businesses.Add(business1);
            businesses.Add(business2);
            businesses.Add(business3);
            businesses.Add(business4);
            businesses.Add(business5);
            businesses.Add(business6);
            businesses.Add(business7);
            businesses.Add(business8);
            businesses.Add(business9);
            businesses.Add(business10);

            UpdateUI(); // Initial UI update to reflect starting state.
            SetupUiUpdateTimer();
            await CheckForAutoSave();
        }

        private void GenericButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && buttonToPurchaseActionMap.ContainsKey(button.Name))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    // Shift key is held down, start auto-upgrade
                    currentButtonName = button.Name;
                    upgradeTimer.Start();
                }
                else
                {
                    // No Shift key, perform a single upgrade
                    buttonToPurchaseActionMap[button.Name]?.Invoke();
                }
            }
        }


        private void GenericButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop the auto-upgrade timer if it was started
            upgradeTimer.Stop();
            currentButtonName = string.Empty; // Reset the button name
        }

        private void UpgradeTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (!string.IsNullOrEmpty(currentButtonName) && buttonToPurchaseActionMap.TryGetValue(currentButtonName, out var action))
                {
                    action?.Invoke();
                }
            });
        }

        private Task CheckForAutoSave()
        {
            string autoSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdCapAutosave.adcap");
            if (File.Exists(autoSavePath))
            {
                var result = MessageBox.Show("An autosave file was found. Do you want to load it?", "Autosave found", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    SaveLoad.LoadGame(businesses, autoSavePath);
                }
            }
            return Task.CompletedTask;
        }

        private void SetupUiUpdateTimer()
        {
            uiUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            uiUpdateTimer.Tick += UiUpdateTimer_Tick;
            uiUpdateTimer.Start();
        }

        private void UiUpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateUI(); // Continuously update UI elements based on the current game state.
        }

        private void UpdateUI()
        {
            string CurrencyString = PlayerValues.Money.ToKMBTQ();
            CurrencyLabel.Text = $"Total Revenue: ${CurrencyString}";

            UpdateBusinessUI(business1, B1UpgradeButton, B1OwnedText, B1Rev);
            UpdateBusinessUI(business2, B2UpgradeButton, B2OwnedText, B2Rev);
            UpdateBusinessUI(business3, B3UpgradeButton, B3OwnedText, B3Rev);
            UpdateBusinessUI(business4, B4UpgradeButton, B4OwnedText, B4Rev);
            UpdateBusinessUI(business5, B5UpgradeButton, B5OwnedText, B5Rev);
            UpdateBusinessUI(business6, B6UpgradeButton, B6OwnedText, B6Rev);
            UpdateBusinessUI(business7, B7UpgradeButton, B7OwnedText, B7Rev);
            UpdateBusinessUI(business8, B8UpgradeButton, B8OwnedText, B8Rev);
            UpdateBusinessUI(business9, B9UpgradeButton, B9OwnedText, B9Rev);
            UpdateBusinessUI(business10, B10UpgradeButton, B10OwnedText, B10Rev);
        }

        private void UpdateBusinessUI(Business business, Button upgradeButton, TextBlock ownedText, TextBlock revenueText)
        {
            // Convert the business cost to a metric representation with 2 decimal places
            string CostString = business.Cost.ToKMBTQ();
            upgradeButton.Content = $"(${CostString})";
            upgradeButton.Background = PlayerValues.Money > business.Cost ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)) : new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            ownedText.Text = $"Owned: {business.AmountOwned}";
            if (business.Time > 100)
            {
                string RevenueString = (business.Revenue * business.Multiplier).ToKMBTQ();
                revenueText.Text = $"${RevenueString}";
            }
            else
            {
                string RevenueString = (business.RevenuePerSecond).ToKMBTQ();
                revenueText.Text = $"${RevenueString}/s";
            }
            if (business.AmountOwned == 0)
            {
                string EarningAddString = (business.EarningAdd * business.Multiplier).ToKMBTQ();
                revenueText.Text = $"${EarningAddString}";
            }
        }

        private void UpgradeMenu_Click(object sender, RoutedEventArgs e)
        {
            var upgradeMenu = UpgradeMenu.GetInstance(this);
            upgradeMenu.Owner = this; // Set the owner to MainWindow
            ViewboxMain.Opacity = 0.5; // Show the overlay
            BackgroundImage.Opacity = 0.5;
            upgradeMenu.ShowDialog();
            ViewboxMain.Opacity = 1;
            BackgroundImage.Opacity = 1;
        }

        public void ApplyUpgrade(string description)
        {
            // Existing patterns
            var businessPattern = @"(Triple|Multiply) Business (?<businessId>\d+) profit( by (?<multiplier>\d+))?";
            var allBusinessPattern = @"(Triple|Multiply) All Businesses profit( by (?<multiplier>\d+))?";
            // New pattern for "Upgrade level effectiveness +x%"
            var effectivenessPattern = @"Upgrade level effectiveness \+(?<effectiveness>\d+)%";

            var businessMatch = Regex.Match(description, businessPattern);
            var allBusinessMatch = Regex.Match(description, allBusinessPattern);
            var effectivenessMatch = Regex.Match(description, effectivenessPattern);

            if (businessMatch.Success)
            {
                // Apply business-specific upgrade
                int businessId = int.Parse(businessMatch.Groups["businessId"].Value);
                int multiplier = businessMatch.Groups["multiplier"].Success ? int.Parse(businessMatch.Groups["multiplier"].Value) : 3; // Default to 3 if not specified
                ApplyBusinessUpgrade(businessId, multiplier);
            }
            else if (allBusinessMatch.Success)
            {
                // Apply upgrade to all businesses
                int multiplier = allBusinessMatch.Groups["multiplier"].Success ? int.Parse(allBusinessMatch.Groups["multiplier"].Value) : 3; // Default to 3 if not specified
                ApplyAllBusinessesUpgrade(multiplier);
            }
            else if (effectivenessMatch.Success)
            {
                // Apply upgrade to level effectiveness
                double effectiveness = double.Parse(effectivenessMatch.Groups["effectiveness"].Value) / 100.0; // Convert percentage to decimal
                ApplyEffectivenessUpgrade(effectiveness);
            }
            else
            {
                MessageBox.Show("Invalid upgrade description.");
            }
        }

        private void ApplyBusinessUpgrade(int businessId, int multiplier)
        {
            Business targetBusiness = GetBusinessById(businessId);
            if (targetBusiness != null)
            {
                targetBusiness.Multiplier *= multiplier;
                targetBusiness.RevenuePerSecond = targetBusiness.CalculateRevenuePerSecond();
                ShowPopup($"Business {businessId}'s profit multiplied by {multiplier}.", 1500);
            }
            else
            {
                MessageBox.Show($"No business found for ID: {businessId}");
            }
        }

        private void ApplyAllBusinessesUpgrade(int multiplier)
        {
            // Apply the multiplier to all businesses
            foreach (var business in businesses)
            {
                business.Multiplier *= multiplier;
            }

            ShowPopup($"All businesses' profits multiplied by {multiplier}.", 1500);
        }

        private void ApplyEffectivenessUpgrade(double effectivenessIncrease)
        {
            PlayerValues.PrestigeLevelsMultiplier += effectivenessIncrease;
            ShowPopup($"Level effectiveness increased by {effectivenessIncrease * 100}%.", 1500);
        }

        private Business GetBusinessById(int id)
        {
            switch (id)
            {
                case 1: return business1;
                case 2: return business2;
                case 3: return business3;
                case 4: return business4;
                case 5: return business5;
                case 6: return business6;
                case 7: return business7;
                case 8: return business8;
                case 9: return business9;
                case 10: return business10;
                default: return null;
            }
        }


        public void ApplyMultiplier(string name, double multiplier)
        {
            switch (name)
            {
                case "Business1":
                    business1.Multiplier *= multiplier;
                    break;
                case "Business2":
                    business2.Multiplier *= multiplier;
                    break;
                case "Business3":
                    business3.Multiplier *= multiplier;
                    break;
                case "Business4":
                    business4.Multiplier *= multiplier;
                    break;
                case "Business5":
                    business5.Multiplier *= multiplier;
                    break;
                case "Business6":
                    business6.Multiplier *= multiplier;
                    break;
                case "Business7":
                    business7.Multiplier *= multiplier;
                    break;
                case "Business8":
                    business8.Multiplier *= multiplier;
                    break;
                case "Business9":
                    business9.Multiplier *= multiplier;
                    break;
                case "Business10":
                    business10.Multiplier *= multiplier;
                    break;
                default:
                    throw new ArgumentException("Unknown business name", nameof(name));
            }
        }

        private void PrestigeMenu_Click(object sender, RoutedEventArgs e)
        {
            // Assuming PrestigeMenu is the class name of your Prestige window
            // and you have a similar GetInstance method as in your UpgradeMenu
            var prestigeMenu = PrestigeMenu.GetInstance(this);
            ViewboxMain.Opacity = 0.5;
            BackgroundImage.Opacity = 0.5;
            prestigeMenu.ShowDialog();
            ViewboxMain.Opacity = 1;
            BackgroundImage.Opacity = 1;
        }

        private void Unlocks_Click(object sender, RoutedEventArgs e)
        {
            var businessUnlocks = BusinessUnlocks.GetInstance();
            businessUnlocks.Owner = this; // Set the owner to MainWindow
            ViewboxMain.Opacity = 0.5; // Show the overlay
            BackgroundImage.Opacity = 0.5;
            businessUnlocks.ShowDialog();
            ViewboxMain.Opacity = 1;
            BackgroundImage.Opacity = 1;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveLoad.SaveGame(GetAllBusinesses());
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            SaveLoad.LoadGame(GetAllBusinesses());
        }

        public async void ResetAndApplyPrestige()
        {
            business1.Reset();
            business2.Reset();
            business3.Reset();
            business4.Reset();
            business5.Reset();
            business6.Reset();
            business7.Reset();
            business8.Reset();
            business9.Reset();
            business10.Reset();

            ApplyPrestigeMultiplier();

            await Task.Delay(250);

            PlayerValues.Money = 5;

            UpdateUI();
        }

        public static void ResetAll()
        {
            foreach (Business business in businesses)
            {
                business.Reset();
            }
        }

        private void ApplyPrestigeMultiplier()
        {
            double prestigeMultiplier = 1 + (PlayerValues.PrestigeLevels * PlayerValues.PrestigeLevelsMultiplier);

            // Assuming each business class has a method to update its profit multiplier
            business1.UpdateProfitWithPrestige(prestigeMultiplier);
            business2.UpdateProfitWithPrestige(prestigeMultiplier);
            business3.UpdateProfitWithPrestige(prestigeMultiplier);
            business4.UpdateProfitWithPrestige(prestigeMultiplier);
            business5.UpdateProfitWithPrestige(prestigeMultiplier);
            business6.UpdateProfitWithPrestige(prestigeMultiplier);
            business7.UpdateProfitWithPrestige(prestigeMultiplier);
            business8.UpdateProfitWithPrestige(prestigeMultiplier);
            business9.UpdateProfitWithPrestige(prestigeMultiplier);
            business10.UpdateProfitWithPrestige(prestigeMultiplier);
        }

        public void ShowPopup(string message, int displayTimeMilliseconds)
        {
            var popup = new PopupWindow(this);
            popup.ShowMessage(message, displayTimeMilliseconds);
            openPopups.Add(popup); // Add the popup to the list of open pop-ups
            popup.Closed += (sender, args) => openPopups.Remove(popup); // Remove from list when closed
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Close all open pop-up windows
            foreach (var popup in openPopups)
            {
                popup.Close();
            }
            openPopups.Clear();
        }

        private void SetupAutoSaveTimer()
        {
            Timer autoSaveTimer = new Timer(300000); // 300,000 ms = 5 minutes
            autoSaveTimer.Elapsed += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    // Define the autosave file path. You could also use a dedicated autosave directory.
                    string autoSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdCapAutosave.adcap");
                    SaveLoad.SaveGame(GetAllBusinesses(), autoSavePath);
                });
            };
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Enabled = true;
        }

    }
}
