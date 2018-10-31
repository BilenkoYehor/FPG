using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace TPSShooter
{

    public class BoughtWeaponPreview : MonoBehaviour
    {

        [Header("BulletsData")]
        // Price for bullets
        public int bulletsPrice = 5;
        // Bullets count that will be added when the player buy bullets
        public int bulletsAddCount = 30;

        [Header("Sounds")]
        public AudioSource goodClick;
        public AudioSource badClick;

        [Header("Weapons")]
        /*
         * Weapons that player's can have
         * Weapons must contain WeaponPrice script
         */
        public GameObject[] weapons;

        [Header("UITexts")]
        public Text cashText;
        public Text weaponNameText;
        public Text weaponInformationText;

        public Text playOrBuyButtonText;

        [Header("UI GameObject")]
        public GameObject previousButton;
        public GameObject nextButton;

        int currentWeaponIndex;

        void Start()
        {
            // Set all GameObjects weapon invisible
            foreach (GameObject g in weapons)
                g.SetActive(false);

            // Set currentWeaponIndex (show weapon that was in the last game)
            //currentWeaponIndex = 0;
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i].tag.Equals(SaveLoad.GetCurrentWeapon()))
                {
                    currentWeaponIndex = i;

                    break;
                }
                currentWeaponIndex = 0;
            }

            // Set current Cash
            UpdateCashText();

            // Updatee current weapon information
            UpdateWeapon();

            // makes invisible in order to Menu screen was visible
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Buy bullets if player has bought the weapon. (Button)
        /// </summary>
        public void BuyBullets()
        {
            WeaponInf w = SaveLoad.GetBoughtWeapon(weapons[currentWeaponIndex].tag);

            if (w != null)
            {
                if (SaveLoad.PlayerCash >= bulletsPrice)
                {
                    goodClick.Play();

                    w.BulletsCount += bulletsAddCount;

                    SaveLoad.PlayerCash = SaveLoad.PlayerCash - bulletsPrice;
                    SaveLoad.UpdateWeapon(w);

                    UpdateWeapon();
                    UpdateCashText();
                }
                else
                {
                    badClick.Play();
                }
            }
            else
            {
                badClick.Play();
            }

        }

        /// <summary>
        /// When press PlayOrBuy button
        /// If the weapon is bought, starts loading The play scene
        /// else buy the weapon if the player has enough cash
        /// If the player does not have enough cash, show AdsGameObject
        /// </summary>
        /// 
        /// <remarks>
        /// (Button)
        /// </remarks>
        public void PlayOrBuy()
        {
            WeaponInf w = SaveLoad.GetBoughtWeapon(weapons[currentWeaponIndex].tag);

            if (w != null)
            {
                goodClick.Play();

                SaveLoad.SetCurrentWeapon(weapons[currentWeaponIndex].tag);
                SaveLoad.SavePlayerPreferences();

                DownloadPlayScene();
            }
            else if (SaveLoad.PlayerCash >= weapons[currentWeaponIndex].GetComponent<WeaponPrice>().price)
            {
                goodClick.Play();

                WeaponInf newWeapon = new WeaponInf()
                {
                    Tag = weapons[currentWeaponIndex].tag
                };
                SaveLoad.AddNewBoughtWeapon(newWeapon);
                SaveLoad.PlayerCash = SaveLoad.PlayerCash - weapons[currentWeaponIndex].GetComponent<WeaponPrice>().price;

                UpdateCashText();
                UpdateWeapon();
            }
            else
            {
                if (SaveLoad.IsSoundOn)
                    badClick.Play();
            }
        }

        [Header("DownloadingPreview")]
        public GameObject downloadingPreview;
        public Text progressText;
        public Slider progressBar;

        AsyncOperation ao;

        /// <summary>
        /// Loads play scene.
        /// </summary>
        public void DownloadPlayScene()
        {
            downloadingPreview.SetActive(true);
            StartCoroutine(LoadLevel());
            progressBar.value = 0;
        }

        /// <summary>
        /// Coroutine for load Play Scene with progress.
        /// </summary>
        IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(0.1f);

            ao = SceneManager.LoadSceneAsync(SaveLoad.CurrentScene);
            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                progressText.text = string.Format("{0}%", Mathf.FloorToInt(ao.progress * 100));
                progressBar.value = Mathf.FloorToInt(ao.progress * 100);

                if (ao.progress >= 0.9f)
                {
                    progressBar.value = 100;
                    progressText.text = string.Format("{0}%", 100);
                    ao.allowSceneActivation = true;

                    gameObject.SetActive(false);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sets next GameObject weapon active. (Button)
        /// </summary>
        public void ShowNextWeapon()
        {
            goodClick.Play();

            currentWeaponIndex++;

            if (currentWeaponIndex <= weapons.Length - 1)
            {
                weapons[currentWeaponIndex - 1].SetActive(false);
            }
            else
            {
                currentWeaponIndex = 0;
                weapons[weapons.Length - 1].SetActive(false);
            }

            UpdateWeapon();
        }

        /// <summary>
        /// Sets previous GameObject weapon active. (Button)
        /// </summary>
        public void ShowPreviousWeapon()
        {
            goodClick.Play();
            currentWeaponIndex--;

            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weapons.Length - 1;
                weapons[0].SetActive(false);
            }
            else
            {
                weapons[currentWeaponIndex + 1].SetActive(false);
            }

            UpdateWeapon();
        }

        /// <summary>
        /// Updates information about the weapon.
        /// </summary>
        void UpdateWeapon()
        {
            // Set currentWeapon active
            weapons[currentWeaponIndex].SetActive(true);
            // Update weaponName text
            weaponNameText.text = string.Format("{0}", weapons[currentWeaponIndex].tag);

            // If the weapon is bought, change information about the weapon
            WeaponInf w = SaveLoad.GetBoughtWeapon(weapons[currentWeaponIndex].tag);
            if (w != null)
            {
                weaponInformationText.text = string.Format("Bullets: {0}", w.BulletsCount + w.BulletsInMagazine);
                playOrBuyButtonText.text = "Play";
            }
            else
            {
                weaponInformationText.text = string.Format("Price: {0}$", weapons[currentWeaponIndex].GetComponent<WeaponPrice>().price);
                playOrBuyButtonText.text = "Buy";
            }

            previousButton.SetActive(true);
            nextButton.SetActive(true);

            if (currentWeaponIndex == 0)
                previousButton.SetActive(false);
            else if (currentWeaponIndex == weapons.Length - 1)
                nextButton.SetActive(false);
        }

        /// <summary>
        /// Updates the cash text.
        /// </summary>
        void UpdateCashText()
        {
            cashText.text = string.Format("Cash: {0}$", SaveLoad.PlayerCash);
        }

    }
}