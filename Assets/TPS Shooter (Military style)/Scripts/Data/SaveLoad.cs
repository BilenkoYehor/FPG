using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TPSShooter
{

    public static class SaveLoad
    {

        static string playerPreferencesPath = "/playerPreferences.pf";

        // Player Preferences
        static PlayerPreferences playerPreferences = new PlayerPreferences();

        /// <summary>
        /// Current Play Scene.
        /// </summary>
        public static int CurrentScene
        {
            get { return playerPreferences.currentScene; }
            set { playerPreferences.currentScene = value; }
        }

        /// <summary>
        /// Touchpad sensitivity.
        /// </summary>
        public static float TouchpadSensitivity
        {
            get { return playerPreferences.touchpadSensitivity; }
            set { playerPreferences.touchpadSensitivity = value; }
        }

        /// <summary>
        /// Aiming sensitivity.
        /// </summary>
        public static float TouchpadAimingSensitivity
        {
            get { return playerPreferences.touchpadAimingSensitivity; }
            set { playerPreferences.touchpadAimingSensitivity = value; }
        }

        /// <summary>
        /// Sound.
        /// </summary>
        public static bool IsSoundOn
        {
            get { return playerPreferences.isSoundOn; }
            set { playerPreferences.isSoundOn = value; }
        }

        /// <summary>
        /// AutoShoot
        /// </summary>
        public static bool IsAutoShoot
        {
            get { return playerPreferences.isAutoShoot; }
            set { playerPreferences.isAutoShoot = value; }
        }

        /// <summary>
        /// Player's cash.
        /// </summary>
        public static int PlayerCash
        {
            get { return playerPreferences.cash; }
            set { playerPreferences.cash = value; }
        }

        /// <summary>
        /// Adds the new bought weapon.
        /// </summary>
        public static void AddNewBoughtWeapon(WeaponInf w)
        {
            playerPreferences.boughtWeapons.Add(w);
        }

        /// <summary>
        /// Sets the current weapon.
        /// </summary>
        public static void SetCurrentWeapon(string weaponTag)
        {
            playerPreferences.currentWeapon = weaponTag;
        }

        /// <summary>
        /// Gets the current weapon.
        /// </summary>
        public static string GetCurrentWeapon()
        {
            return playerPreferences.currentWeapon;
        }

        /// <summary>
        /// Gets bought weapon by Tag.
        /// </summary>
        /// 
        /// <returns>Null if player does not have a such weapon else weapon</returns>
        public static WeaponInf GetBoughtWeapon(string weaponTag)
        {
            foreach (WeaponInf w in playerPreferences.boughtWeapons)
            {
                if (weaponTag.Equals(w.Tag))
                {
                    return w;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the weapon in player's bought weapons.
        /// </summary>
        public static void UpdateWeapon(WeaponInf w)
        {
            WeaponInf newWeapon = GetBoughtWeapon(w.Tag);
            playerPreferences.boughtWeapons.Remove(newWeapon);
            AddNewBoughtWeapon(w);
        }

        /// <summary>
        /// Saves the player's preferences.
        /// </summary>
        public static void SavePlayerPreferences()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + playerPreferencesPath);
            bf.Serialize(file, SaveLoad.playerPreferences);
            file.Close();
        }

        /// <summary>
        /// Loads the player's preferences.
        /// </summary>
        public static void LoadPlayerPreferences()
        {
            if (File.Exists(Application.persistentDataPath + playerPreferencesPath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + playerPreferencesPath, FileMode.Open);
                SaveLoad.playerPreferences = (PlayerPreferences)bf.Deserialize(file);
                file.Close();
            }
        }
    }
}