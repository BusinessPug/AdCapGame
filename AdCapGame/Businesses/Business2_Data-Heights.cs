using System.Windows;

namespace AdCapGame
{
    public class Business2 : Business
    {
        public Business2(IBusinessManager businessManager) : base(businessManager, initialCost: 60, costCoefficient: 1.15, time: 3000, earningAdd: 60)
        {
        }


        protected override async Task StartGeneratingRevenueAsync()
        {
            if (Time < 100)
            {
                if (!isGeneratingRevenuePerSecond)
                {
                    isGeneratingRevenuePerSecond = true;
                    await GenerateRevenuePerSecondAsync();
                }
                else
                    RevenuePerSecond = CalculateRevenuePerSecond();
            }
            else
                await GenerateRevenueWithProgressBarAsync();
        }

        public override async Task GenerateRevenuePerSecondAsync()
        {
            RevenuePerSecond = CalculateRevenuePerSecond();
            Application.Current.Dispatcher.Invoke(() => UpdateProgressBar(100)); // Immediate full progress
            while (isGeneratingRevenue && AmountOwned > 0)
            {
                PlayerValues.AddEarnings(RevenuePerSecond / 5); // Give 1/5 of the revenue per second every 200 ms
                await Task.Delay(200); // Wait for 200ms before the next addition
                if (!isGeneratingRevenue) break;
            }
        }

        public override double CalculateRevenuePerSecond()
        {
            // Calculate how many times the operation can occur in one second
            double operationsPerSecond = 1000.0 / Time;
            // Calculate revenue per second
            return (operationsPerSecond * Revenue) * Multiplier;
        }

        public override async Task GenerateRevenueWithProgressBarAsync()
        {
            if (Time < 100) return;
            while (isGeneratingRevenue && AmountOwned > 0)
            {
                for (int progress = 0; progress <= 100; progress++)
                {
                    if (Time < 100) return; // Exit if time is adjusted mid-operation
                    Application.Current.Dispatcher.Invoke(() => UpdateProgressBar(progress));
                    await Task.Delay((int)(Time / 100)); // Simulate time passage
                    if (!isGeneratingRevenue) break;
                }
                PlayerValues.AddEarnings(Revenue * Multiplier); // Adjust revenue calculation by the multiplier
            }
        }

        protected override void UpdateProgressBar(int progress)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.B2Prog.Value = progress;
                }
            });
        }

        // Define the special unlocks specific to Business2
        protected override Dictionary<int, (SpecialUnlockType, string, double)> SpecialUnlocks => new Dictionary<int, (SpecialUnlockType, string, double)>
        {
            { 125, (SpecialUnlockType.Multiplier, "Business1", 2) }, 
            { 150, (SpecialUnlockType.Multiplier, "Business3", 2) }, 
            { 175, (SpecialUnlockType.Multiplier, "Business4", 2) }, 
            { 225, (SpecialUnlockType.Multiplier, "Business5", 2) }, 
            { 250, (SpecialUnlockType.Multiplier, "Business1", 3) }, 
            { 275, (SpecialUnlockType.Multiplier, "Business3", 3) }, 
            { 300, (SpecialUnlockType.Multiplier, "Business2", 2) }, 
            { 325, (SpecialUnlockType.Multiplier, "Business4", 3) }, 
            { 350, (SpecialUnlockType.Multiplier, "Business5", 3) }, 
            { 375, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 400, (SpecialUnlockType.Multiplier, "Business2", 2) }, 
            { 425, (SpecialUnlockType.Multiplier, "Business3", 4) }, 
            { 450, (SpecialUnlockType.Multiplier, "Business4", 4) }, 
            { 475, (SpecialUnlockType.Multiplier, "Business5", 4) }, 
            { 500, (SpecialUnlockType.Multiplier, "Business6", 11) },
            { 525, (SpecialUnlockType.Multiplier, "Business1", 5) }, 
            { 550, (SpecialUnlockType.Multiplier, "Business3", 5) }, 
            { 575, (SpecialUnlockType.Multiplier, "Business4", 5) }, 
            { 600, (SpecialUnlockType.Multiplier, "Business7", 11) },
            { 625, (SpecialUnlockType.Multiplier, "Business5", 5) }, 
            { 650, (SpecialUnlockType.Multiplier, "Business1", 6) }, 
            { 675, (SpecialUnlockType.Multiplier, "Business3", 6) }, 
            { 700, (SpecialUnlockType.Multiplier, "Business8", 11) },
            { 725, (SpecialUnlockType.Multiplier, "Business4", 6) }, 
            { 750, (SpecialUnlockType.Multiplier, "Business5", 6) }, 
            { 775, (SpecialUnlockType.Multiplier, "Business1", 3) }, 
            { 800, (SpecialUnlockType.Multiplier, "Business9", 11) },
            { 825, (SpecialUnlockType.Multiplier, "Business3", 7) }, 
            { 850, (SpecialUnlockType.Multiplier, "Business4", 7) }, 
            { 875, (SpecialUnlockType.Multiplier, "Business5", 7) }, 
            { 900, (SpecialUnlockType.Multiplier, "Business10", 11) },
            { 925, (SpecialUnlockType.Multiplier, "Business6", 7) }, 
            { 950, (SpecialUnlockType.Multiplier, "Business7", 7) }, 
            { 975, (SpecialUnlockType.Multiplier, "Business8", 7) }, 
            { 1000, (SpecialUnlockType.Multiplier, "Business2", 7777777) }, 
            { 1025, (SpecialUnlockType.Multiplier, "Business9", 7) },
            { 1050, (SpecialUnlockType.Multiplier, "Business10", 7) }, 
            { 1075, (SpecialUnlockType.Multiplier, "Business3", 8) }, 
            { 1100, (SpecialUnlockType.Multiplier, "Business4", 8) }, 
            { 1125, (SpecialUnlockType.Multiplier, "Business5", 8) }, 
            { 1150, (SpecialUnlockType.Multiplier, "Business6", 8) }, 
            { 1175, (SpecialUnlockType.Multiplier, "Business7", 81200) },
            { 1225, (SpecialUnlockType.Multiplier, "Business9", 8) }, 
            { 1250, (SpecialUnlockType.Multiplier, "Business10", 8) },
            { 1300, (SpecialUnlockType.Multiplier, "Business2", 7777) },
            { 1350, (SpecialUnlockType.Multiplier, "Business1", 9) }, 
            { 1400, (SpecialUnlockType.Multiplier, "Business3", 9) }, 
            { 1450, (SpecialUnlockType.Multiplier, "Business4", 9) }, 
            { 1500, (SpecialUnlockType.Multiplier, "Business5", 9) }, 
            { 1550, (SpecialUnlockType.Multiplier, "Business6", 9) }, 
            { 1600, (SpecialUnlockType.Multiplier, "Business7", 9) }, 
            { 1650, (SpecialUnlockType.Multiplier, "Business8", 9) }, 
            { 1700, (SpecialUnlockType.Multiplier, "Business9", 9) }, 
            { 1750, (SpecialUnlockType.Multiplier, "Business10", 9) },
            { 1800, (SpecialUnlockType.Multiplier, "Business6", 10) },
            { 1850, (SpecialUnlockType.Multiplier, "Business7", 10) },
            { 1900, (SpecialUnlockType.Multiplier, "Business8", 10) },
            { 1950, (SpecialUnlockType.Multiplier, "Business9", 10) },
            { 2000, (SpecialUnlockType.Multiplier, "Business2", 7777) },
            { 2100, (SpecialUnlockType.Multiplier, "Business3", 15) }, 
            { 2200, (SpecialUnlockType.Multiplier, "Business4", 15) }, 
            { 2300, (SpecialUnlockType.Multiplier, "Business5", 15) }, 
            { 2400, (SpecialUnlockType.Multiplier, "Business6", 15) }, 
            { 2500, (SpecialUnlockType.Multiplier, "Business2", 777) },
            { 2600, (SpecialUnlockType.Multiplier, "Business8", 15) }, 
            { 2700, (SpecialUnlockType.Multiplier, "Business9", 15) }, 
            { 2800, (SpecialUnlockType.Multiplier, "Business10", 15) },
            { 2900, (SpecialUnlockType.Multiplier, "Business1", 15) }, 
            { 3000, (SpecialUnlockType.Multiplier, "Business2", 777) },
            { 3100, (SpecialUnlockType.Multiplier, "Business3", 20) }, 
            { 3200, (SpecialUnlockType.Multiplier, "Business7", 20) }, 
            { 3300, (SpecialUnlockType.Multiplier, "Business9", 20) }, 
            { 3400, (SpecialUnlockType.Multiplier, "Business10", 20) },
            { 3500, (SpecialUnlockType.Multiplier, "Business2", 777) },
            { 3600, (SpecialUnlockType.Multiplier, "Business7", 25) }, 
            { 3700, (SpecialUnlockType.Multiplier, "Business8", 25) }, 
            { 3800, (SpecialUnlockType.Multiplier, "Business9", 25) }, 
            { 3900, (SpecialUnlockType.Multiplier, "Business10", 25) },
            { 4000, (SpecialUnlockType.Multiplier, "Business2", 777) },
            { 4100, (SpecialUnlockType.Multiplier, "Business1", 30) }, 
            { 4200, (SpecialUnlockType.Multiplier, "Business3", 30) }, 
            { 4300, (SpecialUnlockType.Multiplier, "Business4", 30) }, 
            { 4400, (SpecialUnlockType.Multiplier, "Business5", 30) }, 
            { 4500, (SpecialUnlockType.Multiplier, "Business6", 30) }, 
            { 4600, (SpecialUnlockType.Multiplier, "Business7", 30) }, 
            { 4700, (SpecialUnlockType.Multiplier, "Business8", 30) }, 
            { 4800, (SpecialUnlockType.Multiplier, "Business9", 30) }, 
            { 4900, (SpecialUnlockType.Multiplier, "Business10", 30) },
            { 5000, (SpecialUnlockType.Multiplier, "Business2", 50) }, 
            { 5100, (SpecialUnlockType.Multiplier, "Business2", 50) }, 
            { 5200, (SpecialUnlockType.Multiplier, "Business2", 50) }, 
            { 5300, (SpecialUnlockType.Multiplier, "Business2", 50) }, 
            { 5400, (SpecialUnlockType.Multiplier, "Business2", 50) }
        };

        public override void Reset()
        {
            // stop the value generation methods
            isGeneratingRevenue = false;
            isGeneratingRevenuePerSecond = false;
            Application.Current.Dispatcher.Invoke(() => UpdateProgressBar(0));
            // reset the revenue per second
            RevenuePerSecond = 0;
            // reset the time
            Time = 3000;
            // reset the amount owned
            AmountOwned = 0;
            // reset the revenue
            Revenue = 0;
            // reset the multiplier
            Multiplier = 1;
            // reset the cost
            Cost = 60;
        }
    }
}
