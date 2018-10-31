using UnityEngine;

public class DesktopInputController : InputController
{

    public override void UpdateVariables()
    {
        base.UpdateVariables();

        movementVertical = Input.GetAxis("Vertical");
        movementHorizontal = Input.GetAxis("Horizontal");

        rotationVertical = Input.GetAxis("Mouse Y");
        rotationHorizontal = Input.GetAxis("Mouse X");

        if (Input.GetKeyDown(KeyCode.R))
            isReloadPressed = true;

        if (Input.GetMouseButtonDown(1))
            isScopePressed = true;

        if (Input.GetKeyDown(KeyCode.Space))
            isJumpPressed = true;

        if (Input.GetKey(KeyCode.LeftShift))
            isRunning = true;
        else
            isRunning = false;

        if (Input.GetMouseButton(0))
            isFirePressed = true;
        else
            isFirePressed = false;

        if (Input.GetKeyDown(KeyCode.K))
            isWeaponChanged = true;
    }

}
