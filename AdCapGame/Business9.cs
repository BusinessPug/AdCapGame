﻿using System.Windows;

namespace AdCapGame
{
    public class Business9 : Business
    {
        public Business9(IBusinessManager businessManager) : base(businessManager, initialCost: 2149908480, costCoefficient: 1.08, time: 6144000, earningAdd: 1074954240)
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
                    mainWindow.B9Prog.Value = progress;
                }
            });
        }

        protected override Dictionary<int, (SpecialUnlockType, string, double)> SpecialUnlocks => new Dictionary<int, (SpecialUnlockType, string, double)>
        {
            { 500, (SpecialUnlockType.Multiplier, "Business9", 2) }, 
            { 600, (SpecialUnlockType.Multiplier, "Business9", 2) }, 
            { 700, (SpecialUnlockType.Multiplier, "Business9", 2) }, 
            { 800, (SpecialUnlockType.Multiplier, "Business9", 2) }, 
            { 900, (SpecialUnlockType.Multiplier, "Business9", 2) }, 
            { 1000, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 1100, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1200, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1300, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1400, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1500, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1600, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1700, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1800, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 1900, (SpecialUnlockType.Multiplier, "Business9", 2) },
            { 4250, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 4500, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 4750, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 7000, (SpecialUnlockType.Multiplier, "Business9", 5) },
            { 7250, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 7500, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 7750, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 8000, (SpecialUnlockType.Multiplier, "Business9", 5) },
            { 8250, (SpecialUnlockType.Multiplier, "Business9", 3) },
            { 8500, (SpecialUnlockType.Multiplier, "Business9", 3) },

            { 2250, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 2500, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 2750, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 3000, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 3250, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 3500, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 3750, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
            { 4000, (SpecialUnlockType.TimeHalve, "Business9", 0.5) },
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
            Time = 6144000;
            // reset the amount owned
            AmountOwned = 0;
            // reset the revenue
            Revenue = 0;
            // reset the multiplier
            Multiplier = 1;
            // reset the cost
            Cost = 2149908480;
        }
    }
}