using UnityEngine;

public class MobileInputController : InputController, IGameNotifier
{
    
    [Header("Joystick and Touchpad GameObjects")]
    public JirtualJoystick joystick;
    public Touchpad[] touchpads;

    [Header("UI")]
    public GameObject[] uiObjects;
    
    public override void UpdateVariables()
    {
        base.UpdateVariables();

        // Rotation
        foreach (Touchpad t in touchpads)
        {
            if (t.IsPressed)
            {
                rotationVertical = t.VertivalValue;
                rotationHorizontal = t.HorizontalValue;

                break;
            }
            else
            {
                rotationHorizontal = 0;
                rotationVertical = 0;
            }
        }

        // Movement
        movementHorizontal = joystick.HorizontalValue();
        movementVertical = joystick.VerticalValue();
        isRunning = joystick.IsRun();
    }

    public void ScopePressed()
    {
        isScopePressed = true;
    }

    public void Jump()
    {
        isJumpPressed = true;
    }

    public void Reload()
    {
        isReloadPressed = true;
    }

    public void FirePressedTrue()
    {
        isFirePressed = true;
    }

    public void FirePressedFalse()
    {
        isFirePressed = false;
    }

    public void ChangeWeaponPressed()
    {
        isWeaponChanged = true;
    }

    public void OnGameStopped()
    {
        foreach (GameObject obj in uiObjects)
            obj.SetActive(false);
    }

    public void OnGameResumed()
    {
        foreach (GameObject obj in uiObjects)
            obj.SetActive(true);
    }

    public void OnGameFinished()
    {
        foreach (GameObject obj in uiObjects)
            obj.SetActive(false);
    }
}