using UnityEngine.UI;
using UnityEngine;

namespace TPSShooter
{

    public class LocationPreview : MonoBehaviour
    {

        [Header("Location GameObjects")]
        // each gameObject in locations has to contain LocationInformation script.
        public GameObject[] locations;

        [Header("UI Text")]
        public Text locationInformationText;
        public Text playButtonText;

        [Header("UI Buttons")]
        public GameObject previousButton;
        public GameObject nextButton;

        [Header("Sounds")]
        public AudioSource clickSound;

        [Header("Menu script")]
        public MenuPreview menuScript;

        int currentLocation;

        void Start()
        {
            currentLocation = 0;

            UpdateLocation();

            // makes invisible in order to Menu screen was visible
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Updates the location by given index.
        /// </summary>
        void UpdateLocation()
        {
            foreach (GameObject obj in locations)
                obj.SetActive(false);

            locations[currentLocation].SetActive(true);

            // Updates UI
            locations[currentLocation].GetComponent<LocationInformation>().availableSceneImage.SetActive(true);

            // Updates texts
            locationInformationText.text = locations[currentLocation].GetComponent<LocationInformation>().information;
            playButtonText.text = "Choose";

            // updates visibility of buttons
            previousButton.SetActive(true);
            nextButton.SetActive(true);

            if (currentLocation == 0)
                previousButton.SetActive(false);
            else if (currentLocation == locations.Length - 1)
                nextButton.SetActive(false);
        }

        /// <summary>
        /// Shows buy weapon preview or ad if a current location is locked. (Button)
        /// </summary>
        public void ShowBuyWeaponPreview()
        {
            clickSound.Play();

            SaveLoad.CurrentScene = locations[currentLocation].GetComponent<LocationInformation>().SceneIndex;

            menuScript.BuyWeaponPreviewOn();
        }

        /// <summary>
        /// Shows next location. (Button)
        /// </summary>
        public void NextLocation()
        {
            // Increase current location
            currentLocation++;

            // Checks locations bounds
            if (currentLocation == locations.Length)
                currentLocation = 0;

            UpdateLocation();

            // Play click sound
            clickSound.Play();
        }

        /// <summary>
        /// Shows previous location. (Button)
        /// </summary>
        public void PreviousLocation()
        {
            // Decrease current location
            currentLocation--;
            // Checks bounds
            if (currentLocation == -1)
                currentLocation = locations.Length - 1;

            UpdateLocation();

            // Play click sound
            clickSound.Play();
        }

    }
}