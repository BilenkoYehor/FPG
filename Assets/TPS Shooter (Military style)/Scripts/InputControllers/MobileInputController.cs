using UnityEngine;

namespace TPSShooter
{
    /// <summary>
    /// InputController for mobile platforms that updates all inputController's variables.
    /// </summary>

    public class MobileInputController : InputController
    {

        [Header("All UI GameObjects")]
        // GameObjects that will be shown/hidden when the is Stopped/Resumed/Ended
        // UI gameObjects that have to always be in Scene and does not depend on if the player in the car or out (for example, touchpad)
        public GameObject[] independentGameObjects;
        public GameObject[] playerOutCarObjects;
        public GameObject[] playerInCarObjects;

        [Header("Joystick and Touchpad GameObjects")]
        public VirtualJoystick joystick;
        public Touchpad[] touchpads;

        [Header("Autoshoot")]
        public bool isAutoShoot;
        public GameObject rightFireButton;
        public GameObject leftFireButton;

        [Header("Scope")]
        public GameObject scopeButton;
        bool isScopeAvailable;

//        PlayerBehaviour playerBehaviour;

        public override void Initialize()
        {
            base.Initialize();

            // Add gameNotifier to GameManager
        //    gameManager.AddGameNotifier(this);

            foreach (GameObject obj in playerOutCarObjects)
                obj.SetActive(true);
            availableCarUI.SetActive(false);
            foreach (GameObject obj in playerInCarObjects)
                obj.SetActive(false);

            isAutoShoot = SaveLoad.IsAutoShoot;

            // Autoshoot
            if (isAutoShoot)
            {
                rightFireButton.SetActive(false);
                leftFireButton.SetActive(false);
            }
            else
            {
                rightFireButton.SetActive(true);
                leftFireButton.SetActive(true);
            }

      /*      playerBehaviour = GameObject.FindWithTag(Tags.Player).GetComponent<PlayerBehaviour>();
            if (!playerBehaviour)
                Debug.LogError("MobileInputController: No Player with PlayerBehaviour script was found.");

            // Checks if scope is available and the show
            isScopeAvailable = playerBehaviour.GetCurrentWeaponBehaviour().scopeSettings.IsScopeAvailable;
            */
            ShowScopeButton();
        }

        public override void UpdateVariables()
        {
            base.UpdateVariables();

            // Rotation
            foreach (Touchpad t in touchpads)
            {
                if (t.IsPressed)
                {
                    verticalRotationVar = t.VertivalValue;
                    horizontalRotationVar = t.HorizontalValue;

                    break;
                }
                else
                {
                    horizontalRotationVar = 0;
                    verticalRotationVar = 0;
                }
            }

            // Movement
            horizontalMovementVar = joystick.HorizontalValue();
            verticalMovementVar = joystick.VerticalValue();
            isRun = joystick.IsRun();

            // Is Fire
            if (isAutoShoot)
                if (hitObjectTag.Contains(Tags.Enemy))
                    isFirePressed = true;
                else
                    isFirePressed = false;
        }

        // If the scope is available, scope button is visible
        void ShowScopeButton()
        {
            if (isScopeAvailable)
                scopeButton.SetActive(true);
            else
                scopeButton.SetActive(false);
        }

        public override void StopResume()
        {
            if (gameManager.IsGameStopped)
            {
                gameManager.ResumeGame();
            }
            else
            {
                gameManager.StopGame();
            }
        }

        public override void PlayerGetInCar()
        {
            foreach (GameObject obj in playerInCarObjects)
                obj.SetActive(true);

            foreach (GameObject obj in playerOutCarObjects)
                obj.SetActive(false);
        }

        public override void PlayerGetOutCar()
        {
            foreach (GameObject obj in playerInCarObjects)
                obj.SetActive(false);

            foreach (GameObject obj in playerOutCarObjects)
                obj.SetActive(true);

            ShowScopeButton();
        }

        // Car's variables ⬇ (used in UI gameObjects)
        public void CarFwdActivate()
        {
            verticalCarMovementVar = 1;
        }
        public void CarFwdDeactivate()
        {
            verticalCarMovementVar = 0;
        }
        public void CarBwdActivate()
        {
            verticalCarMovementVar = -1;
        }
        public void CarBwdDeactivate()
        {
            verticalCarMovementVar = 0;
        }
        public void CarRightActivate()
        {
            horizontalCarMovementVar = 1;
        }
        public void CarRightDeactivate()
        {
            horizontalCarMovementVar = 0;
        }
        public void CarLeftActivate()
        {
            horizontalCarMovementVar = -1;
        }

        public void CarLeftDeactivate()
        {
            horizontalCarMovementVar = 0;
        }

        public void BrakePressed()
        {
            isBrakePressed = true;
        }

        public void BrakeDontPressed()
        {
            isBrakePressed = false;
        }
        // Car's variables ⬆ (used in UI gameObjects)

        // Player's variables ⬇ (used in UI gameObjects)
        public void ScopePressed()
        {
            isScopePressed = true;
        }

        public void Crouch()
        {
            isCrouchPressed = true;
        }

        public void GrenadeStart()
        {
            isGrenadeThrowingStart = true;
        }

        public void GrenadeThrow()
        {
            isGrenadeThrow = true;
        }

        public void Jump()
        {
            isJumpPressed = true;
        }

        public void Reload()
        {
            isReloadPressed = true;
        }

        public void Use()
        {
            isUsePressed = true;
        }

        public void FirePressedTrue()
        {
            isFirePressed = true;
        }

        public void FirePressedFalse()
        {
            isFirePressed = false;
        }

        // Player's variables ⬆ (used in UI gameObjects)


        // Makes all UI GameOjbects invisible
        void InvisibleAllUI()
        {
            foreach (GameObject obj in independentGameObjects)
                obj.SetActive(false);

            foreach (GameObject obj in playerOutCarObjects)
                obj.SetActive(false);

            foreach (GameObject obj in playerInCarObjects)
                obj.SetActive(false);
        }

    }
}