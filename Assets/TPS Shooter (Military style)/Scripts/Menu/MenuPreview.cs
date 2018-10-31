using UnityEngine;

namespace TPSShooter
{

    public class MenuPreview : MonoBehaviour
    {

        [Header("Previews")]
        public GameObject homePreview;
        public GameObject settingsPreview;
        public GameObject locationPreview;
        public GameObject buyWeaponPreview;
        public GameObject downloadingPreview;

        [Header("Sounds")]
        public AudioSource click;
        public AudioSource mainSound;

        [Header("Audio Listener")]
        public AudioListener audioListener;

        void Awake()
        {
            // Load playerPreferences to use
            SaveLoad.LoadPlayerPreferences();

            if (!SaveLoad.IsSoundOn)
            {
                // audioListener.enabled = false;
                AudioListener.volume = 0;
            }

            // sets home preview
            homePreview.SetActive(true);

            settingsPreview.SetActive(false);

            // activates buy weapon preview in order to cash and weapon will be initialized (then BoughtWeaponPreview (in Awake method) has to be inactive)
            buyWeaponPreview.SetActive(true);
            // the same reason
            locationPreview.SetActive(true);

            downloadingPreview.SetActive(false);

        }

        /// <summary>
        /// Sets location preview.
        /// </summary>
        public void LocationPreviewOn()
        {
            click.Play();

            locationPreview.SetActive(true);

            homePreview.SetActive(false);
            buyWeaponPreview.SetActive(false);
        }

        /// <summary>
        /// Sets settings preview.
        /// </summary>
        public void SettingsPreviewOn()
        {
            click.Play();

            homePreview.SetActive(false);

            settingsPreview.SetActive(true);
        }

        /// <summary>
        /// Sets buy weapon preview.
        /// </summary>
        public void BuyWeaponPreviewOn()
        {
            click.Play();

            locationPreview.SetActive(false);
            buyWeaponPreview.SetActive(true);
            homePreview.SetActive(false);
            downloadingPreview.SetActive(false);
        }

        /// <summary>
        /// Sets home screen preview.
        /// </summary>
        public void HomePreviewOn()
        {
            click.Play();

            homePreview.SetActive(true);

            locationPreview.SetActive(false);
            settingsPreview.SetActive(false);
            buyWeaponPreview.SetActive(false);
        }

        /// <summary>
        /// Exits the game.
        /// </summary>
        public void Exit()
        {
            click.Play();

            SaveLoad.SavePlayerPreferences();
            Application.Quit();
        }

    }
}