using System.Windows;

namespace AdCapGame
{
    public class Business7 : Business
    {
        public Business7(IBusinessManager businessManager) : base(businessManager, initialCost: 14929920, costCoefficient: 1.10, time: 384000, earningAdd: 7464960)
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
                    mainWindow.B7Prog.Value = progress;
                }
            });
        }

        protected override Dictionary<int, (SpecialUnlockType, string, double)> SpecialUnlocks => new Dictionary<int, (SpecialUnlockType, string, double)>
        {
            { 500, (SpecialUnlockType.Multiplier, "Business7", 2) }, 
            { 600, (SpecialUnlockType.Multiplier, "Business7", 2) }, 
            { 700, (SpecialUnlockType.Multiplier, "Business7", 2) }, 
            { 800, (SpecialUnlockType.Multiplier, "Business7", 2) }, 
            { 900, (SpecialUnlockType.Multiplier, "Business7", 2) }, 
            { 1000, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 1100, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1200, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1300, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1400, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1500, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1600, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1700, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1800, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 1900, (SpecialUnlockType.Multiplier, "Business7", 2) },
            { 2000, (SpecialUnlockType.Multiplier, "Business7", 5) },
            { 2200, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 2400, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 2600, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 2800, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 2900, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 3000, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 3250, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 3500, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 3750, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 4000, (SpecialUnlockType.Multiplier, "Business7", 5) },
            { 4250, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 4500, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 4750, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 5000, (SpecialUnlockType.Multiplier, "Business7", 7) },
            { 5250, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 5500, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 5750, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 6000, (SpecialUnlockType.Multiplier, "Business7", 7) },
            { 6250, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 6500, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 6750, (SpecialUnlockType.Multiplier, "Business7", 3) },
            { 7000, (SpecialUnlockType.Multiplier, "Business7", 7) },

            { 2100, (SpecialUnlockType.TimeHalve, "Business7", 0.5) },
            { 2300, (SpecialUnlockType.TimeHalve, "Business7", 0.5) },
            { 2500, (SpecialUnlockType.TimeHalve, "Business7", 0.5) },
            { 2700, (SpecialUnlockType.TimeHalve, "Business7", 0.5) },
        };

        public override void Reset()
        {
            // stop the value generation methods
            isGeneratingRevenue = false;
            isGeneratingRevenuePerSecond = false;
            // reset the progress bar
            Application.Current.Dispatcher.Invoke(() => UpdateProgressBar(0));
            // reset the revenue per second
            RevenuePerSecond = 0;
            // reset the time
            Time = 384000;
            // reset the amount owned
            AmountOwned = 0;
            // reset the revenue
            Revenue = 0;
            // reset the multiplier
            Multiplier = 1;
            // reset the cost
            Cost = 14929920;
        }
    }
}
