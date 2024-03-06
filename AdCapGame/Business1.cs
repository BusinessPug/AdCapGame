using System.Windows;

namespace AdCapGame
{
    public class Business1 : Business
    {
        public Business1(IBusinessManager businessManager)
            : base(businessManager, initialCost: 3.5, costCoefficient: 1.07, time: 600, earningAdd: 1)
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
            {
                await GenerateRevenueWithProgressBarAsync();
            }
        }

        public override async Task GenerateRevenuePerSecondAsync()
        {
            RevenuePerSecond = CalculateRevenuePerSecond();
            Application.Current.Dispatcher.Invoke(() => UpdateProgressBar(100)); // Immediate full progress
            while (isGeneratingRevenue && AmountOwned > 0)
            {
                PlayerValues.AddEarnings(RevenuePerSecond / 5); // Give 1/5 of the revenue per second every 200 ms
                await Task.Delay(200); // Wait for 200ms before the next addition
                if (!isGeneratingRevenue) 
                    break;
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
                    mainWindow.B1Prog.Value = progress;
                }
            });
        }

        protected override Dictionary<int, (SpecialUnlockType, string, double)> SpecialUnlocks => new Dictionary<int, (SpecialUnlockType, string, double)>
        {
            { 500, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 600, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 700, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 800, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 900, (SpecialUnlockType.Multiplier, "Business1", 4) }, 
            { 1000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 1100, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1200, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1300, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1400, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1500, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1600, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1700, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1800, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 1900, (SpecialUnlockType.Multiplier, "Business1", 4) },
            { 2000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 2250, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 2500, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 2750, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 3000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 3250, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 3500, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 3750, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 4000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 4250, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 4500, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 4750, (SpecialUnlockType.Multiplier, "Business1", 2) },
            { 5000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 5250, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 5500, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 5750, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 6000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 6250, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 6500, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 6750, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 7000, (SpecialUnlockType.Multiplier, "Business1", 5) },
            { 7250, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 7500, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 7777, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 8000, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 8200, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 8400, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 8600, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 8800, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9000, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9100, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9200, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9300, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9400, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9500, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9600, (SpecialUnlockType.Multiplier, "Business1", 3) },   
            { 9700, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9800, (SpecialUnlockType.Multiplier, "Business1", 3) },
            { 9999, (SpecialUnlockType.Multiplier, "Business1", 1.9999) },
            { 10000, (SpecialUnlockType.Multiplier, "Business1", 5) }
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
            Time = 600;
            // reset the amount owned
            AmountOwned = 0;
            // reset the revenue
            Revenue = 0;
            // reset the multiplier
            Multiplier = 1;
            // reset the cost
            Cost = 3.5;
        }
    }
}
