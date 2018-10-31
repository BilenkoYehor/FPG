using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{

    static string playerWeaponPreferencesPath = "/playerPreferences.testWW";
    static string levelPreferencesPath = "/playerPreferences.testLL";
    static string inputPreferencesPath = "/playerPreferences.testII";

    // Preferences
    static LevelPreferences levelPreferences = new LevelPreferences();
    static PlayerWeaponPreferences playerWeaponPreferences = new PlayerWeaponPreferences();
    static InputPreferences inputPreferences = new InputPreferences();

    public static int Cash
    {
        get { return playerWeaponPreferences.cash; }
        set { playerWeaponPreferences.cash = value; }
    }

    public static bool IsWeaponAvailable(int index)
    {
        foreach (int key in playerWeaponPreferences.weaponInformationList.Keys)
            if (key == index ) 
                return true;

        return false;
    }

    public static int CurrentWeaponIndex
    {
        get { return playerWeaponPreferences.currentWeaponIndex; }
        set { playerWeaponPreferences.currentWeaponIndex = value; }
    }

    // Used when the user buys weapon
    public static void AddWeapon(int weaponIndex, int magCapacity, int availableBullets, int bulletsInMag)
    {
        WeaponInf newWeapon = new WeaponInf(magCapacity, availableBullets, bulletsInMag);

        playerWeaponPreferences.weaponInformationList.Add(weaponIndex, newWeapon);
    }

    // These 3 methods used in Play scene, when you want to get data
    public static int CurrentMagCapacity
    {
        get { return playerWeaponPreferences.weaponInformationList[playerWeaponPreferences.currentWeaponIndex].MagazineCapacity; }
    }

    public static int CurrentAvailableBullets
    {
        get { return playerWeaponPreferences.weaponInformationList[playerWeaponPreferences.currentWeaponIndex].AvailableBullets; }
    }

    public static int CurrentBulletsInMag
    {
        get { return playerWeaponPreferences.weaponInformationList[playerWeaponPreferences.currentWeaponIndex].BulletsInMag; }
    }

    // Used while buying bullets or after the player win/lose the game
    public static void UpdateWeaponInformation(int weaponIndex, int availableBullets, int bulletsInMag)
    {
        WeaponInf updatedWeapon = playerWeaponPreferences.weaponInformationList[weaponIndex];
        updatedWeapon.AvailableBullets = availableBullets;
        updatedWeapon.BulletsInMag = bulletsInMag;

        playerWeaponPreferences.weaponInformationList[weaponIndex] = updatedWeapon;
    }

    public static int AvailableLevel
    {
        get { return levelPreferences.availableLevel; }
        set { levelPreferences.availableLevel = value; }
    }

    public static int CurrentLevel
    {
        get { return levelPreferences.currentLevel; }
        set { levelPreferences.currentLevel = value; }
    }

    public static bool IsAutoShoot
    {
        get { return inputPreferences.isAutoShoot; }
        set { inputPreferences.isAutoShoot = value; }
    }

    public static float Sensitivity
    {
        get { return inputPreferences.sensitivity; }
        set { inputPreferences.sensitivity = value; }
    }

    public static float FireButtonSensitivity
    {
        get { return inputPreferences.fireButtonSensitivity; }
        set { inputPreferences.fireButtonSensitivity = value; }
    }

    public static float AimingSensitivity
    {
        get { return inputPreferences.aimingSensitivity; }
        set { inputPreferences.aimingSensitivity = value; }
    }

    /// <summary>
    /// Saves the player's weapon preferences.
    /// </summary>
    public static void SavePlayerWeaponPreferences()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + playerWeaponPreferencesPath);
        bf.Serialize(file, SaveLoad.playerWeaponPreferences);
        file.Close();
    }

    /// <summary>
    /// Loads the player's weapon preferences.
    /// </summary>
    public static void LoadPlayerWeaponPreferences()
    {
        if (File.Exists(Application.persistentDataPath + playerWeaponPreferencesPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + playerWeaponPreferencesPath, FileMode.Open);
            SaveLoad.playerWeaponPreferences = (PlayerWeaponPreferences)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void SaveLevelPreferences()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + levelPreferencesPath);
        bf.Serialize(file, SaveLoad.levelPreferences);
        file.Close();
    }

    public static void LoadLevelPreferences()
    {
        if (File.Exists(Application.persistentDataPath + levelPreferencesPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + levelPreferencesPath, FileMode.Open);
            SaveLoad.levelPreferences = (LevelPreferences)bf.Deserialize(file);
            file.Close();
        }
    }

    public static void SaveInputPreferences()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + inputPreferencesPath);
        bf.Serialize(file, SaveLoad.inputPreferences);
        file.Close();
    }

    public static void LoadInputPreferences()
    {
        if (File.Exists(Application.persistentDataPath + inputPreferencesPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + inputPreferencesPath, FileMode.Open);
            SaveLoad.inputPreferences = (InputPreferences)bf.Deserialize(file);
            file.Close();
        }
    }
}