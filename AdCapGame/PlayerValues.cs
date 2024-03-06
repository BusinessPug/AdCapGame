public static class PlayerValues
{
    public static double Money { get; set; }
    public static double MoneyPerSecond { get; set; }
    public static double LifetimeEarnings { get; set; } // Tracks the total money earned over all time
    public static double PrestigeLevels { get; set; } // Tracks the current number of Prestige Levels
    public static double StartingLifetimeEarnings { get; set; } // The amount of Lifetime Earnings at the last reset
    public static double PrestigeLevelsMultiplier { get; set; } = 0.02; // Each level increases profits by 2%

    // Method to add earnings, which also updates Lifetime Earnings
    public static void AddEarnings(double amount)
    {
        Money += amount;
        LifetimeEarnings += amount;
    }

    // Call this method when the player spends money, to ensure that only "earned" money is considered
    public static void SpendMoney(double amount)
    {
        Money -= amount;
    }

    public static void ResetForPrestige()
    {
        Money = 0;
        MoneyPerSecond = 0;
        // Do not reset LifetimeEarnings here as it is used for calculating prestige levels
    }
}
