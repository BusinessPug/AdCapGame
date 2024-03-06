using AdCapGame;

public struct Upgrade
{
    public string Id;
    public string Description;
    public string CostText;
    public double CostValue;
    public Action<IBusinessManager> Apply;
}
