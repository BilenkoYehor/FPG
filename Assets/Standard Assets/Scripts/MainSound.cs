using UnityEngine;

public class MainSound : MonoBehaviour, IGameNotifier {

    AudioSource audioSource;

    public void OnGameFinished()
    {
    }

    public void OnGameResumed()
    {
        audioSource.Play();
    }

    public void OnGameStopped()
    {
        audioSource.Stop();
    }

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	

}
