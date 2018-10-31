using UnityEngine;

public class Crosshair : MonoBehaviour, IScopeNotifier {
    
    public void ScopeOff()
    {
        gameObject.SetActive(true);
    }

    public void ScopeOn()
    {
        gameObject.SetActive(false);
    }

}