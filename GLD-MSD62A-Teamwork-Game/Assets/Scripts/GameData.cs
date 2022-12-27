public static class GameData
{
    private static float _money;

    public static float Money
    {
        get { return _money; }
        set { _money = value; }
    }

    private static int _invetoryAmmo;

    public static int InventoryAmmo
    {
        get { return _invetoryAmmo; }
        set { _invetoryAmmo = value; }
    }
}
