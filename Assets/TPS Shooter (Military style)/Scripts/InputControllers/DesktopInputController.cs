using UnityEngine;
using UnityEngine.UI;

namespace TPSShooter
{

    /// <summary>
    /// InputController for Desktop platforms that updates all inputController's variables.
    /// </summary>

    public class DesktopInputController : InputController, IGameNotifier
    {

        [Header("Sensitivity")]
        public float mouseSensitivity = 10;

        [Header("Key codes")]
        public KeyCode runKeyCode = KeyCode.LeftShift;
        public KeyCode jumpKeyCode = KeyCode.Space;
        public KeyCode reloadKeyCode = KeyCode.R;
        public KeyCode stopGameKeyCode = KeyCode.Escape;
        public KeyCode scopeKeyCode = KeyCode.O;
        public KeyCode grenadeKeyCode = KeyCode.G;
        public KeyCode crouchKeyCode = KeyCode.C;
        public KeyCode carBrakeKeyCode = KeyCode.B;
        public KeyCode useKeyCode = KeyCode.F;

        [Header("UI")]
        public Text availableCarText;

        public override void Initialize()
        {
            // calls base method
            base.Initialize();

            // Add gameNotifier to GameManager
            gameManager.AddGameNotifier(this);

            // Makes cursor invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Debug.Log("DesktopInputController: Press Key: " + jumpKeyCode + " to jump.");
            Debug.Log("DesktopInputController: Press Key: " + reloadKeyCode + " to reload.");
            Debug.Log("DesktopInputController: Press Key: " + stopGameKeyCode + " to stop the game.");
            Debug.Log("DesktopInputController: Press Key: " + scopeKeyCode + " to scope (if scope available).");
            Debug.Log("DesktopInputController: Press Key: " + grenadeKeyCode + " to throw grenade.");
            Debug.Log("DesktopInputController: Press Key: " + crouchKeyCode + " to crouch.");
            Debug.Log("DesktopInputController: Press Key: " + carBrakeKeyCode + " to brake.");
            Debug.Log("DesktopInputController: Press Key: " + useKeyCode + " to use.");
        }

        public override void UpdateVariables()
        {
            base.UpdateVariables();

            // Movement
            horizontalMovementVar = Input.GetAxis("Horizontal");
            verticalMovementVar = Input.GetAxis("Vertical");

            // Car movement
            horizontalCarMovementVar = horizontalMovementVar;
            verticalCarMovementVar = verticalMovementVar;

            // Rotation
            horizontalRotationVar = Input.GetAxis("Mouse X") * mouseSensitivity;
            verticalRotationVar = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Is Fire
            if (Input.GetMouseButton(0))
                isFirePressed = true;
            else
                isFirePressed = false;

            // Is Jump
            if (Input.GetKeyDown(jumpKeyCode))
                isJumpPressed = true;

            // Is Reload
            if (Input.GetKeyDown(reloadKeyCode))
                isReloadPressed = true;

            // Stop/Resume the game
            if (Input.GetKeyDown(stopGameKeyCode))
                StopResume();

            // Scope
            if (Input.GetKeyDown(scopeKeyCode))
                isScopePressed = true;

            // Grenade
            if (Input.GetKeyDown(grenadeKeyCode))
                isGrenadeThrowingStart = true;
            else if (Input.GetKeyUp(grenadeKeyCode))
                isGrenadeThrow = true;

            // Crouch
            if (Input.GetKeyDown(crouchKeyCode))
                isCrouchPressed = true;

            if (Input.GetKey(runKeyCode))
            {
                isRun = true;

            }
            else
            {
                isRun = false;
            }

            // Brake
            if (Input.GetKeyDown(carBrakeKeyCode))
            {
                isBrakePressed = true;
            }
            else if (Input.GetKeyUp(carBrakeKeyCode))
            {
                isBrakePressed = false;
            }

            // Use (for example, get in the car)
            if (Input.GetKeyDown(useKeyCode))
                isUsePressed = true;
        }

        // Shows available car UI
        public override void ShowAvailableCarUI()
        {
            base.ShowAvailableCarUI();

            availableCarText.text = string.Format("Press {0} to get in", useKeyCode);
        }

        // Stops or resumes the game depending on the game state
        public override void StopResume()
        {
            if (gameManager.IsGameStopped)
            {
                gameManager.ResumeGame();

                // Sets cursor invisible
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                gameManager.StopGame();

                // sets cursor visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void IGameNotifier.GameResumed()
        {
            // do nothing
        }

        void IGameNotifier.GameStopped()
        {
            // do nothing
        }

        void IGameNotifier.GameEnded()
        {
            // Sets cursor visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            availableCarUI.SetActive(false);
        }

        // When the player gets in the car Car UI becomes invisible
        public override void PlayerGetInCar()
        {
            DisappearAvailableCarUI();
        }

        public override void PlayerGetOutCar()
        {
            // do nothing
        }
    }
}