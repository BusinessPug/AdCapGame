using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace AdCapGame
{
    public class SaveLoad
    {
        public static void SaveGame(List<Business> businesses, string filePath = null)
        {
            // If no filePath is provided, use SaveFileDialog to get user-selected path
            if (string.IsNullOrEmpty(filePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "AdCap (*.adcap)|*.adcap",
                    Title = "Save AdCap Game"
                };
                if (saveFileDialog.ShowDialog() != true) return; // User cancelled or closed the dialog
                filePath = saveFileDialog.FileName;
            }

            try
            {
                var businessAttributes = new List<dynamic>();


                foreach (var business in businesses)
                {
                    businessAttributes.Add(new
                    {
                        Time = business.Time,
                        Revenue = business.Revenue,
                        Multiplier = business.Multiplier,
                        AmountOwned = business.AmountOwned,
                        Cost = business.Cost,
                        isGeneratingRevenue = business.isGeneratingRevenue

                    });
                }

                var SaveData = new
                {
                    Money = PlayerValues.Money,
                    MoneyPerSecond = PlayerValues.MoneyPerSecond, // Save this if relevant
                    LifetimeEarnings = PlayerValues.LifetimeEarnings,
                    PrestigeLevels = PlayerValues.PrestigeLevels,
                    StartingLifetimeEarnings = PlayerValues.StartingLifetimeEarnings,
                    PurchasedUpgrades = UpgradeMenu.purchasedUpgrades.ToList(),
                    BusinessAttributes = businessAttributes,
                };

                string json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        public static async void LoadGame(List<Business> businesses, string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "AdCap (*.adcap)|*.adcap",
                    Title = "Load AdCap Game"
                };

                if (openFileDialog.ShowDialog() != true)
                {
                    return; // User cancelled or closed the dialog, exit the method
                }
                filePath = openFileDialog.FileName;
            }
            try
            {
                // Directly use the filePath (either provided or chosen through the dialog)
                string json = File.ReadAllText(filePath);
                dynamic SaveData = JsonConvert.DeserializeObject(json);
                
                MainWindow.ResetAll();

                await Task.Delay(250); // Simulate some loading delay

                PlayerValues.Money = SaveData.Money;
                PlayerValues.MoneyPerSecond = SaveData.MoneyPerSecond;
                PlayerValues.LifetimeEarnings = SaveData.LifetimeEarnings;
                PlayerValues.PrestigeLevels = SaveData.PrestigeLevels;
                PlayerValues.StartingLifetimeEarnings = SaveData.StartingLifetimeEarnings;
                UpgradeMenu.purchasedUpgrades = new HashSet<string>(SaveData.PurchasedUpgrades.ToObject<List<string>>());

                var businessAttributes = SaveData.BusinessAttributes.ToObject<List<dynamic>>();

                for (int i = 0; i < 10; i++)
                {
                    var attributes = businessAttributes[i];
                    var business = businesses[i];

                    business.Time = (double)attributes.Time;
                    business.Revenue = (double)attributes.Revenue;
                    business.Multiplier = (double)attributes.Multiplier;
                    business.AmountOwned = (int)attributes.AmountOwned;
                    business.Cost = (double)attributes.Cost;
                    business.isGeneratingRevenue = (bool)attributes.isGeneratingRevenue;

                    if ((bool)business.isGeneratingRevenue && business.AmountOwned > 0)
                    {
                        if (business.Time < 100)
                        {
                            business.GenerateRevenuePerSecondAsync(); // not awaited as that breaks the for loop. to be fixed later
                        }
                        else
                        {
                            business.GenerateRevenueWithProgressBarAsync(); // same as above
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}");
            }
        }
    }
}