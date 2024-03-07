using System.ComponentModel;
using System.Windows;

namespace AdCapGame
{
    public abstract class Business : INotifyPropertyChanged
    {
        private int amountOwned;
        private double revenue;
        private double cost;
        private readonly double initialTime;
        private double time;
        private double earningAdd;
        private readonly double costCoefficient;
        public bool isGeneratingRevenue = false;
        public bool isGeneratingRevenuePerSecond = false;
        private double multiplier = 1.0; // Initial multiplier value
        private bool TestBool = false;
        private double revenuePerSecond;
        private double baseProfit; // Your base profit for the business
        public double ProfitMultiplier { get; private set; } = 1.0;
        public abstract Task GenerateRevenuePerSecondAsync();
        public abstract Task GenerateRevenueWithProgressBarAsync();
        protected abstract Task StartGeneratingRevenueAsync();
        protected abstract void UpdateProgressBar(int progress);
        public abstract void Reset();
        protected abstract Dictionary<int, (SpecialUnlockType, string, double)> SpecialUnlocks { get; }
        public abstract double CalculateRevenuePerSecond();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly IBusinessManager businessManager;

        protected Business(IBusinessManager businessManager, double initialCost, double costCoefficient, double time, double earningAdd)
        {
            this.businessManager = businessManager; // Save the businessManager instance
            this.cost = initialCost;
            this.costCoefficient = costCoefficient;
            this.initialTime = time;
            this.time = time;
            this.earningAdd = earningAdd;
            UpdateProfitWithPrestige(); // Ensure multiplier is correct at instantiation
        }

        public bool IsGeneratingRevenue
        {
            get => isGeneratingRevenue;
            set
            {
                if (isGeneratingRevenue != value)
                {
                    isGeneratingRevenue = value;
                    OnPropertyChanged(nameof(IsGeneratingRevenue));
                }
            }
        }

        public int AmountOwned
        {
            get => amountOwned;
            set
            {
                if (amountOwned != value)
                {
                    amountOwned = value;
                    OnPropertyChanged(nameof(AmountOwned));
                    AdjustTimeForMilestones();
                }
            }
        }

        public double Revenue
        {
            get => revenue;
            set
            {
                if (Math.Abs(revenue - value) > 0.01)
                {
                    revenue = value;
                    OnPropertyChanged(nameof(Revenue));
                }
            }
        }

        public double EarningAdd
        {
            get => earningAdd;
            set
            {
                if (Math.Abs(earningAdd - value) > 0.01)
                {
                    earningAdd = value;
                    OnPropertyChanged(nameof(EarningAdd));
                }
            }
        }

        public double Cost
        {
            get => cost;
            set
            {
                if (Math.Abs(cost - value) > 0.01)
                {
                    cost = value;
                    OnPropertyChanged(nameof(Cost));
                }
            }
        }

        public double Time
        {
            get => time;
            set
            {
                if (Math.Abs(time - value) > 0.01)
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public double Multiplier
        {
            get => multiplier;
            set
            {
                if (multiplier != value)
                {
                    multiplier = value;
                    OnPropertyChanged(nameof(Multiplier));
                }
            }
        }

        public double RevenuePerSecond
        {
            get => revenuePerSecond;
            set
            {
                if (Math.Abs(revenuePerSecond - value) > 0.01)
                {
                    revenuePerSecond = value;
                    OnPropertyChanged(nameof(RevenuePerSecond));
                }
            }
        }

        public void UpdateProfitWithPrestige()
        {
            // This recalculates the multiplier based on Prestige Levels
            this.multiplier *= (1 + PlayerValues.PrestigeLevels * PlayerValues.PrestigeLevelsMultiplier);
        }

        public async Task Purchase()
        {
            if (PlayerValues.Money >= cost)
            {
                AmountOwned++;
                PlayerValues.SpendMoney(cost);
                cost *= costCoefficient;
                Revenue += earningAdd; // this gets multiplied by the multiplier at a different stage
                if (!isGeneratingRevenue)
                {
                    isGeneratingRevenue = true;
                    await StartGeneratingRevenueAsync();
                }
                if (Time < 100)
                {
                    revenuePerSecond = CalculateRevenuePerSecond();
                    await StartGeneratingRevenueAsync();
                }
            }
        }

        protected void AdjustTimeForMilestones()
        {
            var milestones = new[] { 25, 50, 100, 200, 300, 400 };
            if (milestones.Contains(AmountOwned))
            {
                Time /= 2;
                // `this` refers to the current business instance
                ShowPopupMessage($"Time halved for {this.GetType().Name}!");
            }
            ApplySpecialUnlocks();
        }

        protected void ApplySpecialUnlocks()
        {
            if (SpecialUnlocks.TryGetValue(AmountOwned, out var unlockInfo))
            {

                switch (unlockInfo.Item1)
                {
                    case SpecialUnlockType.Multiplier:
                        businessManager.ApplyMultiplier(unlockInfo.Item2, unlockInfo.Item3);
                        ShowPopupMessage($"Profit multiplied by {unlockInfo.Item3} for {unlockInfo.Item2}!");
                        break;
                    case SpecialUnlockType.TimeHalve:
                        Time *= unlockInfo.Item3; // unlockInfo.Item3 is expected to be 0.5 for halving the time, but perhaps will be dynamic at a later date
                        ShowPopupMessage($"Time halved for {unlockInfo.Item2}!");
                        break;
                }
            }
        }

        protected void ShowPopupMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PopupWindow popup = new PopupWindow(Application.Current.MainWindow.Owner);
                popup.ShowMessage(message, 2000);

                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // Position popup to the top right of the main window
                    popup.Left = mainWindow.Left + mainWindow.Width - popup.Width - 30; // 30 pixels margin from the right
                    popup.Top = mainWindow.Top + 40; // 40 pixels margin from the top
                }
            });
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void UpdateProfitWithPrestige(double prestigeLevels)
        {
            Multiplier = prestigeLevels;
        }

    }
}
