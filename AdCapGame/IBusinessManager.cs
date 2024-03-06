namespace AdCapGame
{
    public interface IBusinessManager
    {
        abstract void ApplyUpgrade(string name);
        abstract void ApplyMultiplier(string name, double multiplier);
    }
}
