using AdCapGame;

public class PrestigeManager
{
    public const double DifficultyFactor = 400e12; //

    public static double CalculateNewPrestigeLevels()
    {
        double currentEarnings = PlayerValues.LifetimeEarnings;
        double startingEarnings = PlayerValues.StartingLifetimeEarnings;
        double newLevels = (currentEarnings / Math.Pow(DifficultyFactor / 9, 0.5)) - (startingEarnings / Math.Pow(DifficultyFactor / 9, 0.5));
        return Math.Floor(newLevels); // Round down to the nearest whole number
    }

    public delegate void ResetGameHandler();
    public static event ResetGameHandler OnResetGame;

    public static void ResetGame()
    {
        var newPrestigeLevels = PrestigeManager.CalculateNewPrestigeLevels();
        PlayerValues.ResetForPrestige();
        PlayerValues.StartingLifetimeEarnings = PlayerValues.LifetimeEarnings;
        PlayerValues.PrestigeLevels += newPrestigeLevels;
        // clear the HashSet of purchased upgrades
        UpgradeMenu.purchasedUpgrades.Clear();
        OnResetGame?.Invoke();
    }
}
