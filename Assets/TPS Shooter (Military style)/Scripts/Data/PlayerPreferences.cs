using System.Collections.Generic;

namespace TPSShooter
{

    [System.Serializable]
    public class PlayerPreferences
    {

        // Player's preferences
        public int cash = 700;
        public string currentWeapon;
        public List<WeaponInf> boughtWeapons = new List<WeaponInf>();

        // Game preferences
        public bool isSoundOn = true;

        // Available scenes
        public int currentScene;

        // Mobile preferences
        public bool isAutoShoot;
        public float touchpadSensitivity = 30;
        public float touchpadAimingSensitivity = 14;

    }
}