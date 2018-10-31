using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject[] gameNotifiersObjects;
    IGameNotifier[] gameNotifiers;

    [Header("- UI without stop -")]
    public GameObject stopButton;

    [Header("- Pause UI -")]
    public GameObject pauseUI;

    [Header("- Loading UI -")]
    public GameObject[] loadingObjects;
    public Slider loadingBar;
    public Text loadingText;

	// Use this for initialization
	void Start () {
        gameNotifiers = new IGameNotifier[gameNotifiersObjects.Length];

        for (int i = 0; i < gameNotifiersObjects.Length; i++)
        {
            gameNotifiers[i] = gameNotifiersObjects[i].GetComponent<IGameNotifier>();
        }
	}

    public void StopGame()
    {
        Time.timeScale = 0;

        stopButton.SetActive(false);

        pauseUI.SetActive(true);
        
        foreach (IGameNotifier notifier in gameNotifiers)
        {
            notifier.OnGameStopped();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        stopButton.SetActive(true);

        pauseUI.SetActive(false);
        
        foreach (IGameNotifier notifier in gameNotifiers)
        {
            notifier.OnGameResumed();
        }
    }

    public void Replay()
    {
        Time.timeScale = 1;

        foreach (GameObject obj in loadingObjects)
            obj.SetActive(true);
        
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;

        foreach (GameObject obj in loadingObjects)
            obj.SetActive(true);

        StartCoroutine(LoadLevel(0));
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

    public void FinishGame()
    {
        foreach (IGameNotifier notifier in gameNotifiers)
        {
            notifier.OnGameFinished();
        }
    }

}