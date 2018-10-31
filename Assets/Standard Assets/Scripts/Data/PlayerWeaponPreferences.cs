using System.Collections.Generic;

[System.Serializable]
public class PlayerWeaponPreferences{

    // Player's preferences
    public int cash = 1200;
    public int currentWeaponIndex = 0;

    public Dictionary<int, WeaponInf> weaponInformationList = new Dictionary<int, WeaponInf>();
}
