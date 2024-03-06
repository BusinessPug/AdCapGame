using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace AdCapGame
{
    public class SaveLoad
    {
        public static void SaveGame(List<Business> businesses)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "AdCap (*.adcap)|*.adcap",
                Title = "Save AdCap Game"
            };

            try
            {
                if (saveFileDialog.ShowDialog() == true)
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
                    File.WriteAllText(saveFileDialog.FileName, json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

        public static async void LoadGame(List<Business> businesses)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "AdCap (*.adcap)|*.adcap",
                Title = "Load AdCap Game"
            };

            try
            {
                if (openFileDialog.ShowDialog() == true)
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    dynamic SaveData = JsonConvert.DeserializeObject(json);

                    MainWindow.ResetAll();

                    await Task.Delay(250);

                    PlayerValues.Money = SaveData.Money;
                    PlayerValues.MoneyPerSecond = SaveData.MoneyPerSecond; // Assuming you want to load this
                    PlayerValues.LifetimeEarnings = SaveData.LifetimeEarnings;
                    PlayerValues.PrestigeLevels = SaveData.PrestigeLevels;
                    PlayerValues.StartingLifetimeEarnings = SaveData.StartingLifetimeEarnings;
                    UpgradeMenu.purchasedUpgrades = new HashSet<string>(SaveData.PurchasedUpgrades.ToObject<List<string>>());

                    var businessAttributes = SaveData.BusinessAttributes.ToObject<List<dynamic>>();


                    for (int i = 0; i < businesses.Count; i++)
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
                                await business.GenerateRevenuePerSecondAsync();
                            }
                            else
                            {
                                await business.GenerateRevenueWithProgressBarAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}");
            }
        }

    }
}