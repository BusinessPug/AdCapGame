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

    /// <summary>
    /// This is just a mess. The IBusinessManager interface is being worked on.
    /// TODO:
    /// Remove IBusinessManager from MainWindow class
    /// Move upgrade logic from MainWindow to a new UpgradeManager class
    /// The UpgradeStruct thus needs to be reworked as well, along with all other upgrade logic
    /// Prestige Level Effectiveness needs to be implemented
    /// Move other non UI based logic away from MainWindow
    /// General Cleanup
    /// restore OOP principles to the project
    /// </summary>

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
        private Dictionary<string, Func<int, Task>> buttonToPurchaseActionMap;
        private string upgradeAmount = "Upgrade 1x";
        private string CostString;
        private double CostValue;
        private Dictionary<string, Business> buttonNameToBusinessMap;
        public List<Business> GetAllBusinesses() => new List<Business> { business1, business2, business3, business4, business5, business6, business7, business8, business9, business10 };

        // maybe a little too disorienting of a variable list


        public MainWindow() // a lot here needs cleaning
        {
            InitializeComponent();
            PlayerValues.Money = 5;
            InitializeGame();

            PopulatePurchaseToAction();

            PopulateButtonToBusiness();

            upgradeTimer = new DispatcherTimer();
            upgradeTimer.Interval = TimeSpan.FromMilliseconds(50);
            upgradeTimer.Tick += UpgradeTimer_Tick;

            PrestigeManager.OnResetGame += ResetAndApplyPrestige;
            SetupAutoSaveTimer();
            this.Closing += MainWindow_Closing;
        }

        private async void InitializeGame() // Oh boy. gonna have to figure out the removal of IBusinessManager here.
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

            AddBusinessesToList();

            UpdateUI(); // Initial UI update to reflect starting state.
            SetupUiUpdateTimer();
            await CheckForAutoSave();
        }

        private void PopulatePurchaseToAction() // There is most likely a better way to do this
        {
            buttonToPurchaseActionMap = new Dictionary<string, Func<int, Task>>
            {
                { "B1UpgradeButton", async (amount) => await business1.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B2UpgradeButton", async (amount) => await business2.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B3UpgradeButton", async (amount) => await business3.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B4UpgradeButton", async (amount) => await business4.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B5UpgradeButton", async (amount) => await business5.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B6UpgradeButton", async (amount) => await business6.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B7UpgradeButton", async (amount) => await business7.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B8UpgradeButton", async (amount) => await business8.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B9UpgradeButton", async (amount) => await business9.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) },
                { "B10UpgradeButton", async (amount) => await business10.Purchase(amount).ContinueWith(task => UpdateUI(), TaskScheduler.FromCurrentSynchronizationContext()) }
            };
        }

        private void PopulateButtonToBusiness()
        {
            buttonNameToBusinessMap = new Dictionary<string, Business>
            {
                { "B1UpgradeButton", business1 },
                { "B2UpgradeButton", business2 },
                { "B3UpgradeButton", business3 },
                { "B4UpgradeButton", business4 },
                { "B5UpgradeButton", business5 },
                { "B6UpgradeButton", business6 },
                { "B7UpgradeButton", business7 },
                { "B8UpgradeButton", business8 },
                { "B9UpgradeButton", business9 },
                { "B10UpgradeButton", business10 }
            };
        }

        private void AddBusinessesToList()
        {
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
        }

        private void GenericButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) // this is a mess to read, but it works so it stays for now
        {
            if (sender is Button button)
            {
                currentButtonName = button.Name;

                // Capture the mouse to the button
                button.CaptureMouse();

                // Check if the Shift key is held down and start the timer
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    // Start your repeating action
                    upgradeTimer.Start();
                }
                else
                {
                    // Perform the action once
                    if (buttonToPurchaseActionMap.TryGetValue(currentButtonName, out var action))
                    {
                        if (buttonNameToBusinessMap.TryGetValue(currentButtonName, out Business business))
                        {
                            int amountToPurchase = GetAmountToPurchase(business);
                            if (amountToPurchase > 0)
                            {
                                action.Invoke(amountToPurchase);
                            }
                        }
                    }
                }
            }
        }

        private void GenericButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                // Release the mouse capture
                button.ReleaseMouseCapture();

                // Stop the repeating action
                if (button.Name == currentButtonName)
                {
                    upgradeTimer.Stop();
                    currentButtonName = string.Empty;
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                // Stop the repeating action if the Shift key was released
                if (!string.IsNullOrEmpty(currentButtonName))
                {
                    upgradeTimer.Stop();
                    currentButtonName = string.Empty;
                }
            }
        }

        private int GetAmountToPurchase(Business business) // needs to go into Business class at some point
        {
            int amountToPurchase = 0;
            double totalCost = 0;
            double tempCost = business.Cost;

            switch (upgradeAmount)
            {
                case "Upgrade 1x":
                    amountToPurchase = 1;
                    break;
                case "Upgrade 10x":
                    amountToPurchase = 10;
                    break;
                case "Upgrade 100x":
                    amountToPurchase = 100;
                    break;
                case "Upgrade MAX":
                    while (PlayerValues.Money >= totalCost + tempCost)
                    {
                        amountToPurchase++;
                        totalCost += tempCost;
                        tempCost *= business.costCoefficient;
                    }
                    break;
            }

            return amountToPurchase;
        }

        private void UpgradeTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(async () =>
            {
                if (!string.IsNullOrEmpty(currentButtonName) && buttonToPurchaseActionMap.TryGetValue(currentButtonName, out var action))
                {
                    if (buttonNameToBusinessMap.TryGetValue(currentButtonName, out Business business))
                    {
                        int amountToPurchase = GetAmountToPurchase(business);
                        if (amountToPurchase > 0)
                        {
                            await action.Invoke(amountToPurchase);
                        }
                    }
                }
            });
        }

        private Task CheckForAutoSave() // need to implement the usage of AppData at some point instead of MyDocuments
        {
            string autoSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdCapAutosave.adcap");
            if (File.Exists(autoSavePath))
            {
                var result = MessageBox.Show("An autosave file was found. Do you want to load it?", "Autosave found", MessageBoxButton.YesNo);  // this will need to be changed out for a prettier message box than MessageBox
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

        private void UpdateUI() // most likely a cleaner way to do this with objects. will look into that
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

        private void UpdateBusinessUI(Business business, Button upgradeButton, TextBlock ownedText, TextBlock revenueText) // Gonna be broken up into smaller methods for readability later
        {
            double costForDisplay = 0;
            int maxUpgrades = 0;
            switch (upgradeAmount)
            {
                case "Upgrade 1x":
                    costForDisplay = business.Cost;
                    break;
                case "Upgrade 10x":
                    costForDisplay = UpgradexCost(business, 10);
                    break;
                case "Upgrade 100x":
                    costForDisplay = UpgradexCost(business, 100);
                    break;
                case "Upgrade MAX":
                    maxUpgrades = GetMaxAffordableUpgrades(business);
                    costForDisplay = UpgradexCost(business, maxUpgrades);
                    break;
            }

            CostString = costForDisplay.ToKMBTQ();
            CostValue = costForDisplay;
            if (upgradeAmount == "Upgrade MAX")
            {
                upgradeButton.Content = maxUpgrades > 0 ? $"{maxUpgrades}x (${CostString})" : $"({UpgradexCost(business, 1).ToKMBTQ()})";
                upgradeButton.Background = maxUpgrades > 0 ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)) :
                    new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            }
            else
            {
                upgradeButton.Content = $"(${CostString})";
                upgradeButton.Background = PlayerValues.Money >= CostValue ? new SolidColorBrush(Color.FromArgb(128, 0, 255, 0)) :
                    new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
            }
            ownedText.Text = $"Owned: {business.AmountOwned}";

            // Update revenue text
            string revenueString = business.AmountOwned == 0
                ? (business.EarningAdd * business.Multiplier).ToKMBTQ()
                : (business.Time > 100 ? (business.Revenue * business.Multiplier).ToKMBTQ() : (business.RevenuePerSecond).ToKMBTQ() + "/s");

            revenueText.Text = $"${revenueString}";
        }

        private double UpgradexCost(Business business, int amount) // more logic that needs to be moved
        {
            double totalCost = 0;
            double cost = business.Cost;
            if (amount == 0)
            {
                while (PlayerValues.Money >= cost)
                {
                    PlayerValues.Money -= cost;
                    totalCost += cost;
                    cost *= business.costCoefficient;
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    totalCost += cost;
                    cost *= business.costCoefficient;
                }
            }
            return totalCost;
        }

        private int GetMaxAffordableUpgrades(Business business) // more logic that needs to be moved
        {
            int upgrades = 0;
            double cost = business.Cost;
            double money = PlayerValues.Money;
            while (money >= cost)
            {
                money -= cost;
                cost *= business.costCoefficient;
                upgrades++;
            }
            return upgrades;
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

        public void ApplyUpgrade(string description) // this is gonna get tricky
        {
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

        private void ApplyBusinessUpgrade(int businessId, int multiplier) // why was i doing this in the MainWindow class to begin with?
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

        private void ApplyAllBusinessesUpgrade(int multiplier) // thankfully a straightforward method to move to Business class
        {
            // Apply the multiplier to all businesses
            foreach (var business in businesses)
            {
                business.Multiplier *= multiplier;
            }

            ShowPopup($"All businesses' profits multiplied by {multiplier}.", 1500);
        }

        private void ApplyEffectivenessUpgrade(double effectivenessIncrease) // this probably does work, but isn't implemented yet, so this will be moved at some point and then ill figure out the calls
        {
            PlayerValues.PrestigeLevelsMultiplier += effectivenessIncrease;
            ShowPopup($"Level effectiveness increased by {effectivenessIncrease * 100}%.", 1500);
        }

        private Business GetBusinessById(int id) // this will become redundant with proper OOP
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

        public void ApplyMultiplier(string name, double multiplier) // WHY IS THIS HERE?!
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

        public static void ResetAll() // WHAT WAS I THINKING??
        {
            foreach (Business business in businesses)
            {
                business.Reset();
            }
        }

        private void ApplyPrestigeMultiplier() // IT'S ONLY DOWNHILL FROM HERE
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

        private void SetupAutoSaveTimer() // this shouldn't be in here either
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

        private void UpgradeAmount_Click(object sender, RoutedEventArgs e) // there's a better way to do this
        {
            if (upgradeAmount == "Upgrade 1x")
            {
                upgradeAmount = "Upgrade 10x";
                UpgradeAmountButton.Content = "Upgrade 10x";
            }
            else if (upgradeAmount == "Upgrade 10x")
            {
                upgradeAmount = "Upgrade 100x";
                UpgradeAmountButton.Content = "Upgrade 100x";
            }
            else if (upgradeAmount == "Upgrade 100x")
            {
                upgradeAmount = "Upgrade MAX";
                UpgradeAmountButton.Content = "Upgrade MAX";
            }
            else if (upgradeAmount == "Upgrade MAX")
            {
                upgradeAmount = "Upgrade 1x";
                UpgradeAmountButton.Content = "Upgrade 1x";
            }
        }
    }
}
