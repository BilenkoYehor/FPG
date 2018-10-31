using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TPSShooter
{

    /// <summary>
    /// This class controlls the game.
    /// It can stop/resume/end the game (when the game is stopped/resumed/ended this class notifies other GameObjects that implements IGameNotifier interface).
    /// It can also download Menu scene and Play scene.
    /// </summary>

    public class GameManager : MonoBehaviour
    {

        [Header("Pause and GameOver GameObjects")]
        public GameObject pauseObject;
        public GameObject gameOverObject;

        [Header("Downloading tools")]
        public GameObject downloadObject;
        public Text progressText;
        public Slider progressBar;

        // Game states 
        bool isGameStopped;
        bool isGameEnded;
        bool isNewSceneDownloading;

        // Game game notifiers 
        List<IGameNotifier> gameNotifiers = new List<IGameNotifier>();

        // Uses to save data and checks if the player is alive to call Stop/Resume method
    //    PlayerBehaviour playerBehaviour;
        // Uses for ResumeByButton, to hide all UI elements correctly
        InputController inputController;

        void Start()
        {
            // Checks if the GameObject is tagged as GameManager
            if (!gameObject.tag.Equals(Tags.GameManager))
                Debug.LogError("GameManager: GameManager has to be tagged as " + Tags.GameManager + ".");

            if (!GameObject.FindWithTag(Tags.Player))
                Debug.LogError("GameManager: No Player that has PlayerBehaviour script found in the game.");
           // playerBehaviour = GameObject.FindWithTag(Tags.Player).GetComponent<PlayerBehaviour>();

            if (!GameObject.FindWithTag(Tags.InputController))
                Debug.LogError("GameManager: No InputController that has InputController script found in the game.");
            inputController = GameObject.FindWithTag(Tags.InputController).GetComponent<InputController>();

            // Set all UI gameObjects invisible
            pauseObject.SetActive(false);
            downloadObject.SetActive(false);
            gameOverObject.SetActive(false);
        }

        public bool IsGameStopped
        {
            get { return isGameStopped; }
            set { isGameStopped = value; }
        }

        public bool IsGameEnded
        {
            get { return isGameEnded; }
        }

        /// <summary>
        /// Adds the IGameNotifier.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void AddGameNotifier(IGameNotifier obj)
        {
            gameNotifiers.Add(obj);
        }

        /// <summary>
        /// Calls this method when the Replay button is pressed. (used for UI button)
        /// </summary>
        public void ReplayByButton()
        {
            // Sets the game state
            isNewSceneDownloading = true;

            // Save player data
            SaveData();

            // Set some UI GameObjects visible and some ones invisible
            downloadObject.SetActive(true);
            pauseObject.SetActive(false);
            gameOverObject.SetActive(false);

            // Starts coroutine
            StartCoroutine(LoadPlayScene());
            // Sets progressBar value as zero
            progressBar.value = 0;
        }

        /// <summary>
        /// Calls this method when the Home button is pressed. (used for UI button)
        /// </summary>
        public void GoHomeByButton()
        {
            // Sets the game state
            isNewSceneDownloading = true;

            // Calls this method in order to if the game was stopped, update method would be called
            Time.timeScale = 1f;

            // Save data
            SaveData();
            // Set some UI GameObjects visible and some ones invisible
            downloadObject.SetActive(true);
            pauseObject.SetActive(false);
            gameOverObject.SetActive(false);

            // Starts coroutine
            StartCoroutine(LoadHomeScene());
            // Sets progressBar value as zero
            progressBar.value = 0;
        }

        /// <summary>
        /// Calls this method when the Resume button is pressed. (used for UI button)
        /// </summary>
        public void ResumeByButton()
        {
            inputController.StopResume();
        }

        /// <summary>
        /// Calls this method to finish the game.
        /// </summary>
        public void EndGame()
        {
            // Sets the game state
            isGameEnded = true;

            // Set gameOver UI visibel
            gameOverObject.SetActive(true);

            // Notifies all gameNotifiers
            for (int i = 0; i < gameNotifiers.Count; i++)
            {
                try
                {
                    gameNotifiers[i].GameEnded();
                }
                catch (MissingReferenceException)
                {
                    gameNotifiers.Remove(gameNotifiers[i]);
                }
            }
        }

        /// <summary>
        /// Resume game.
        /// </summary>
        public void ResumeGame()
        {
            // Checks game states in order to not freeze the game
     //       if (!isNewSceneDownloading && playerBehaviour.IsAlive)
       //     {
                Time.timeScale = 1;

                // sets the game state
                isGameStopped = false;

                // sets the Pause GameObject UI invisible
                pauseObject.SetActive(false);

                // notifies all gameNotifiers
                for (int i = 0; i < gameNotifiers.Count; i++)
                {
                    try
                    {
                        gameNotifiers[i].GameResumed();
                    }
                    catch (MissingReferenceException)
                    {
                        gameNotifiers.Remove(gameNotifiers[i]);
                    }
                }
         //   }
        }

        /// <summary>
        /// Stop Game.
        /// </summary>
        public void StopGame()
        {
            // Checks game states in order to not freeze the game
       ///     if (!isNewSceneDownloading && playerBehaviour.IsAlive)
       //     {
                // Freeze the game
                Time.timeScale = 0;

                // sets the game state
                isGameStopped = true;
                // sets the Pause GameObject UI visible
                pauseObject.SetActive(true);

                // notifies all gameNotifiers
                for (int i = 0; i < gameNotifiers.Count; i++)
                {
                    try
                    {
                        gameNotifiers[i].GameStopped();
                    }
                    catch (MissingReferenceException)
                    {
                        gameNotifiers.Remove(gameNotifiers[i]);
                    }
                }
         //   }
        }

        /*
         * Save player's data about cash and the weapon. This method called in SaveLoad 
         */
        /// <summary>
        /// Save player's data about cash and the weapon.
        /// </summary>
        public void SaveData()
        {
            // saves cash data
        /*    SaveLoad.PlayerCash = SaveLoad.PlayerCash + playerBehaviour.EarnedCash;

            // saves weapon data
            WeaponInf w = new WeaponInf()
            {
                Tag = playerBehaviour.GetCurrentWeaponBehaviour().tag,
                BulletsInMagazine = playerBehaviour.GetCurrentWeaponBehaviour().bulletsInMag,
                BulletsCount = playerBehaviour.GetCurrentWeaponBehaviour().bulletsAmount
            };
            SaveLoad.UpdateWeapon(w);

            // saves data in file
            SaveLoad.SavePlayerPreferences();*/
        }

        AsyncOperation ao;

        /// <summary>
        /// Coroutine for load Home Scene with progress.
        /// </summary>
        IEnumerator LoadHomeScene()
        {
            yield return new WaitForSeconds(0.1f);

            ao = SceneManager.LoadSceneAsync(0);
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
                }


                yield return null;
            }
        }

        /// <summary>
        /// Coroutine for load Play Scene with progress.
        /// </summary>
        IEnumerator LoadPlayScene()
        {
            yield return new WaitForSeconds(0.1f);

            ao = SceneManager.LoadSceneAsync(1);
            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                progressText.text = string.Format("{0}%", ao.progress * 100);
                progressBar.value = ao.progress * 100;

                if (ao.progress >= 0.9f)
                {
                    progressBar.value = 100;
                    progressText.text = string.Format("{0}%", 100);
                    ao.allowSceneActivation = true;
                }


                yield return null;
            }
        }

    }
}