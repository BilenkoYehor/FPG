using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    [Header("- Sounds -")]
    public AudioSource goodClick;
    public AudioSource badClick;

    [Header("- Camera settings -")]
    public Transform cameraPos;
    public Vector3 startPos;
    public Vector3 endPos;
    public float lerpSpeed = 0.1f;

    // UI
    [Header("- Main menu -")]
    public GameObject[] mainMenuObjects;

    [Header("- Settings -")]
    public GameObject[] settingsObjects;
    public Toggle autoShootToggle;
    public Slider sensitivitySlider;
    public Slider fireButtonSensitivitySlider;
    public Slider aimingSensitivitySlider;

    [Header("- Buy weapon -")]
    public GameObject[] buyWeaponObjects;
    public GameObject[] weaponObjects;
    string[] weaponNames = { "MP5", "Ak47", "M16" };
    int[] weaponPrices = { 40, 50, 700 };
    // just for show
    int[] weaponMagCapacities = { 30, 45, 30 };
    int[] weaponRates = {23,45,38 };
    int[] weaponDamages = { 2, 4, 7 };
    public Text cashText;
    public Text weaponNameText;
    public Text weaponAvailabilityText;
    public GameObject nextWeaponButton;
    public GameObject previousWeaponButton;
    public Text weaponRateText;
    public Text weaponDamageText;
    public Text weapoonMagText;
    public GameObject bulletsCountObj;
    public Text bulletsCountText;
    public Text playBuyText;

    [Header("- Choose level -")]
    public GameObject[] chooseLevelObjects;
    public GameObject nextLevelButton;
    public GameObject previousLevelButton;
    public Text[] locationButtonTexts;
    public GameObject firstLocationImg;
    public GameObject secondLocationImg;
    public Text locationNameText;
    public Text locationDescriptionText;
    string[] locationName = { "Air Station", "Bunkers and docks" };
    string[] locationDescription = { "This is just first location", "Taht is a second one." };

    [Header("- Loading -")]
    public GameObject[] loadingObjects;
    public Slider loadingBar;
    public Text loadingText;

	void Start () {
        foreach (GameObject obj in mainMenuObjects)
            obj.SetActive(true);

        foreach (GameObject obj in settingsObjects)
            obj.SetActive(false);
        foreach (GameObject obj in buyWeaponObjects)
            obj.SetActive(false);
        foreach (GameObject obj in chooseLevelObjects)
            obj.SetActive(false);
        foreach (GameObject obj in loadingObjects)
            obj.SetActive(false);
	}

    void Update()
    {
        if(isMenu)
        {
            cameraPos.position = Vector3.Lerp(cameraPos.position, startPos, Time.deltaTime * lerpSpeed);
        }
        else
        {
            cameraPos.position = Vector3.Lerp(cameraPos.position, endPos, Time.deltaTime * lerpSpeed);
        }
    }

    bool isMenu = true;

    /// <summary>
    /// SETTINGS MEHTODS
    /// </summary>

    // When Settings button is pressed
    public void SettingsActive()
    {
        goodClick.Play();

        foreach (GameObject obj in settingsObjects)
            obj.SetActive(true);

        foreach (GameObject obj in mainMenuObjects)
            obj.SetActive(false);

        SaveLoad.LoadInputPreferences();

        autoShootToggle.isOn = SaveLoad.IsAutoShoot;
        sensitivitySlider.value = SaveLoad.Sensitivity;
        fireButtonSensitivitySlider.value = SaveLoad.FireButtonSensitivity;
        aimingSensitivitySlider.value = SaveLoad.AimingSensitivity;
    }

    public void AutoShootValueChanged()
    {
        goodClick.Play();
    }

    // When in Settings scene Back button is pressed
    public void SettingsInactive()
    {
        goodClick.Play();

        SaveLoad.IsAutoShoot = autoShootToggle.isOn;
        SaveLoad.Sensitivity = sensitivitySlider.value;
        SaveLoad.FireButtonSensitivity = fireButtonSensitivitySlider.value;
        SaveLoad.AimingSensitivity = aimingSensitivitySlider.value;

        SaveLoad.SaveInputPreferences();

        foreach (GameObject obj in mainMenuObjects)
            obj.SetActive(true);

        foreach (GameObject obj in settingsObjects)
            obj.SetActive(false);
    }

    /// <summary>
    /// BUY WEAPON/BULLETS METHODS
    /// </summary>

    public void BuyWeaponActive()
    {
        goodClick.Play();

        isMenu = false;
        
        foreach (GameObject obj in buyWeaponObjects)
            obj.SetActive(true);

        foreach (GameObject obj in mainMenuObjects)
            obj.SetActive(false);
        foreach (GameObject obj in chooseLevelObjects)
            obj.SetActive(false);

        SaveLoad.LoadPlayerWeaponPreferences();

        cashText.text = SaveLoad.Cash + "$";

        currentWeaponIndex = 0;
        
        UpdateWeapon(currentWeaponIndex);

    }
    int currentWeaponIndex;

    void UpdateWeapon(int index)
    {
        SaveLoad.CurrentWeaponIndex = currentWeaponIndex;

        foreach (GameObject obj in weaponObjects)
            obj.SetActive(false);

        weaponObjects[index].SetActive(true);

        weaponNameText.text = weaponNames[index];
        weaponRateText.text = weaponRates[index].ToString();
        weaponDamageText.text = weaponDamages[index].ToString();
        weapoonMagText.text = weaponMagCapacities[index].ToString();

        if (SaveLoad.IsWeaponAvailable(index))
        {
            weaponAvailabilityText.text = "Available";
            bulletsCountObj.SetActive(true);
            bulletsCountText.text = (SaveLoad.CurrentAvailableBullets + SaveLoad.CurrentBulletsInMag).ToString();

            playBuyText.text = "Play";
        }
        else
        {
            weaponAvailabilityText.text = weaponPrices[index] + "$";
            bulletsCountObj.SetActive(false);

            playBuyText.text = "Buy";

        }

        if (currentWeaponIndex == 0)
            previousWeaponButton.SetActive(false);
        else
        {
            previousWeaponButton.SetActive(true);

            if (currentWeaponIndex == weaponObjects.Length - 1)
                nextWeaponButton.SetActive(false);
            else
                nextWeaponButton.SetActive(true);
        }
    }

    public void NextWeapon()
    {
        goodClick.Play();

        currentWeaponIndex++;
        UpdateWeapon(currentWeaponIndex);
    }

    public void PreviousWeapon()
    {
        goodClick.Play();

        currentWeaponIndex--;
        UpdateWeapon(currentWeaponIndex);
    }

    public void PlayBuyWeapon()
    {
        // Go to Levels scene
        if (SaveLoad.IsWeaponAvailable(currentWeaponIndex))
        {
            goodClick.Play();

            SaveLoad.SavePlayerWeaponPreferences();
            
            foreach (GameObject obj in chooseLevelObjects)
                obj.SetActive(true);

            foreach (GameObject obj in buyWeaponObjects)
                obj.SetActive(false);

            SaveLoad.LoadLevelPreferences();
            availableLevel = SaveLoad.AvailableLevel;
            UpdateLocation();
        }
        // Buy weapon of fail
        else
        {
            if (SaveLoad.Cash >= weaponPrices[currentWeaponIndex])
            {
                goodClick.Play();

                SaveLoad.AddWeapon(currentWeaponIndex, 
                                   weaponMagCapacities[currentWeaponIndex],
                                   weaponMagCapacities[currentWeaponIndex],
                                   weaponMagCapacities[currentWeaponIndex]);

                SaveLoad.Cash -= weaponPrices[currentWeaponIndex];

                cashText.text = SaveLoad.Cash + "$";

                UpdateWeapon(currentWeaponIndex);
            }
            else
            {
                badClick.Play();
            }
        }
    }

    int bulletsPrice = 50;
    public void BuyBullets()
    {
        if(SaveLoad.IsWeaponAvailable(currentWeaponIndex) && SaveLoad.Cash>= bulletsPrice)
        {
            goodClick.Play();

            SaveLoad.UpdateWeaponInformation(currentWeaponIndex, 
                                             SaveLoad.CurrentAvailableBullets + weaponMagCapacities[currentWeaponIndex],
                                             SaveLoad.CurrentBulletsInMag);

            SaveLoad.Cash -= bulletsPrice;

            cashText.text = SaveLoad.Cash + "$";

            bulletsCountText.text = (SaveLoad.CurrentAvailableBullets + SaveLoad.CurrentBulletsInMag).ToString();
        }
        else
        {
            badClick.Play();
        }
    }

    public void BuyWeaponToMenu()
    {
        goodClick.Play();

        isMenu = true;

        SaveLoad.SavePlayerWeaponPreferences();
        
        foreach (GameObject obj in mainMenuObjects)
            obj.SetActive(true);
        
        foreach (GameObject obj in buyWeaponObjects)
            obj.SetActive(false);
    }

    /// <summary>
    /// Location methods
    /// </summary>

    int availableLevel;
    int currentCard;

    public void FirstLevelSelected()
    {
        int i = 1 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void SecondLevelSelected()
    {
        int i = 2 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void ThirdLevelSelected()
    {
        int i = 3 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void FourthLevelSelected()
    {
        int i = 4 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void FifthLevelSelected()
    {
        int i = 5 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void SixthLevelSelected()
    {
        int i = 6 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void SeventhLevelSelected()
    {
        int i = 7 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void EigthLevelSelected()
    {
        int i = 8 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }
    public void NinthLevelSelected()
    {
        int i = 9 + currentCard * locationButtonTexts.Length;
        if (i <= availableLevel)
        {
            goodClick.Play();
            DownloadPlayScene(i);
        }
        else
        {
            badClick.Play();
        }
    }

    void UpdateLocation()
    {
        if (currentCard < 2)
        {
            firstLocationImg.SetActive(true);
            secondLocationImg.SetActive(false);

            locationNameText.text = locationName[0];
            locationDescriptionText.text = locationDescription[0];
        }
        else
        {
            firstLocationImg.SetActive(false);
            secondLocationImg.SetActive(true);

            locationNameText.text = locationName[1];
            locationDescriptionText.text = locationDescription[1];
        }
        
        if (currentCard == 3)
        {
            nextLevelButton.SetActive(false);
        }
        else
        {
            nextLevelButton.SetActive(true);

            if (currentCard == 0)
                previousLevelButton.SetActive(false);
            else
                previousLevelButton.SetActive(true);
        }               

        for (int i = 0; i < locationButtonTexts.Length; i++)
        {
            int levelIndex = (i + 1) + currentCard * locationButtonTexts.Length;

            if (levelIndex <= availableLevel)
                locationButtonTexts[i].text = "Level " + levelIndex;
            else
                locationButtonTexts[i].text = "Locked";
        }
    }

    public void NextLocationCard()
    {
        goodClick.Play();

        currentCard++;

        UpdateLocation();
    }

    public void PreviousLocationCard()
    {
        goodClick.Play();

        currentCard--;

        UpdateLocation();
    }

    public void DownloadPlayScene(int index)
    {
        SaveLoad.CurrentLevel = index;
        SaveLoad.SaveLevelPreferences();

        foreach (GameObject obj in loadingObjects)
            obj.SetActive(true);

        foreach (GameObject obj in chooseLevelObjects)
            obj.SetActive(false);

        if (index < 19)
            StartCoroutine(LoadLevel(1));
        else
            StartCoroutine(LoadLevel(2));
        loadingBar.value = 0;
    }

    /// <summary>
    /// Coroutine for load Play Scene with progress.
    /// </summary>
    AsyncOperation ao;
    IEnumerator LoadLevel(int sceneIndex)
    {
        yield return new WaitForSeconds(0.1f);

        ao = SceneManager.LoadSceneAsync(sceneIndex);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            loadingText.text = string.Format("{0}%", Mathf.FloorToInt(ao.progress * 100));
            loadingBar.value = Mathf.FloorToInt(ao.progress * 100);

            if (ao.progress >= 0.9f)
            {
                loadingBar.value = 100;
                loadingText.text = string.Format("{0}%", 100);
                ao.allowSceneActivation = true;

                gameObject.SetActive(false);
            }

            yield return null;
        }
    }

}