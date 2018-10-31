using UnityEngine;

public class FpsCamera : MonoBehaviour {

    [Header("- Cameras -")]
    public Camera mainCamera;
    public Camera weaponCamera;

    [Header("- FOV -")]
    public float normalFieldOfView = 64;
    public float scopeSpeed = 3f;
    float scopeFieldOfView;

    bool isAiming;
    public bool IsAiming
    {
        set { isAiming = value; }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update () {
        if(isAiming)
        {
            UpdateFOV(scopeFieldOfView);
        }
        else
        {
            UpdateFOV(normalFieldOfView);
        }
	}

    void UpdateFOV(float fieldOfView)
    {
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, fieldOfView, Time.deltaTime * scopeSpeed);
        weaponCamera.fieldOfView = mainCamera.fieldOfView;
    }

    public void SetAimingFOV(float fov)
    {
        scopeFieldOfView = fov;
    }

}
