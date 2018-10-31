using UnityEngine;

namespace TPSShooter
{

    /// <summary>
    /// Basic class that is responsible for inputs.
    /// </summary>

    public abstract class InputController : MonoBehaviour
    {

        [Header("Car UI")]
        // UI GameObject that will be visible when available car is detected
        public GameObject availableCarUI;

        // UI GameObject that will be visible for some period of time when player does not have bullets in magazine
        public GameObject reloadUI;
        public float shownReloadUITime = 2;

        [Header("- Run -")]
        public bool runSupported;

        [Header("- Layers -")]
        public LayerMask shootingLayers;

        protected GameManager gameManager;

        void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            // Finds GameManager
            gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();
            if (!gameManager)
                Debug.LogError("InputController: no GameManager found in the game.");
        }

        /*
         * These variables must be initialized in Update method⬇
         */
        // movement and rotation variables
        protected float verticalMovementVar;
        protected float horizontalMovementVar;
        protected float verticalRotationVar;
        protected float horizontalRotationVar;

        protected bool isRun;

        // car variables
        protected float verticalCarMovementVar;
        protected float horizontalCarMovementVar;
        protected bool isBrakePressed;

        protected bool isUsePressed;

        // player's states
        protected bool isJumpPressed;
        protected bool isFirePressed;
        protected bool isReloadPressed;
        protected bool isScopePressed;
        protected bool isCrouchPressed;

        // when player decides to throw grenade isGrenadeThrowingStart will be tru
        // then the grenade will not be thrown before isGrenadeThrow will be true
        protected bool isGrenadeThrowingStart;
        protected bool isGrenadeThrow;

        // Position where weapon shoots at
        protected Vector3 fireShootAt;
        protected GameObject hitObject;
        protected string hitObjectTag = "";
        protected string hitObjectName = "";

        /*
         * These variables must be initialized in Update method⬆
         */

        void Update()
        {
            UpdateVariables();
        }

        public virtual void UpdateVariables()
        {
            // Hit object Position, hit object Name, hit object Tag, hit GameObject
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, shootingLayers))
            {
                fireShootAt = hit.point;
                hitObject = hit.collider.gameObject;
                hitObjectTag = hit.collider.tag;
                hitObjectName = hit.collider.name;
            }
            else
            {
                fireShootAt = Vector3.zero;
                hitObjectTag = "";
                hitObjectName = "";
            }
        }

        /// <summary>
        /// Reload UI
        /// </summary>
        public void ShowReloadUI()
        {
            reloadUI.SetActive(true);

            isUsedReloadUI = true;

            Invoke("DisableReloadUI", shownReloadUITime);
        }

        bool isUsedReloadUI;

        void DisableReloadUI()
        {
            isUsedReloadUI = false;

            reloadUI.SetActive(false);
        }

        /// <summary>
        /// Car UI
        /// </summary>
        public virtual void ShowAvailableCarUI()
        {
            availableCarUI.SetActive(true);
        }

        public void DisappearAvailableCarUI()
        {
            availableCarUI.SetActive(false);
        }

        // Calls StopGame/ResumeGame methods from GameManager depending on the game state
        public abstract void StopResume();

        // These methods will be called from PlayerBehaviour when the player get in/out the car
        public abstract void PlayerGetInCar();
        public abstract void PlayerGetOutCar();

        // Player's variables ⬇
        public bool IsUsePressed
        {
            get
            {
                if (isUsePressed)
                {
                    isUsePressed = false;
                    return true;
                }

                return false;
            }
        }

        public bool IsGrenadeThrowingStart
        {
            get
            {
                if (isGrenadeThrowingStart)
                {
                    isGrenadeThrowingStart = false;
                    return true;
                }

                return false;
            }
        }

        public bool IsGrenadeThrow
        {
            get
            {
                if (isGrenadeThrow)
                {
                    isGrenadeThrow = false;
                    return true;
                }

                return false;
            }
        }

        public bool IsCrouchPressed
        {
            get
            {
                if (isCrouchPressed)
                {
                    isCrouchPressed = false;
                    return true;
                }

                return false;
            }
        }

        public float VerticalMovementVar
        {
            get { return verticalMovementVar; }
        }

        public float HorizontalMovementVar
        {
            get { return horizontalMovementVar; }
        }

        public bool IsRun
        {
            get { if (runSupported) return isRun; else return false; }
        }

        public float VerticalRotationVar
        {
            get { return verticalRotationVar; }
            set { verticalRotationVar = value; }
        }

        public float HorizontalRotationVar
        {
            get { return horizontalRotationVar; }
            set { horizontalRotationVar = value; }
        }

        public GameObject HitOjbect
        {
            get { return hitObject; }
        }

        public bool IsScopePressed
        {
            get
            {
                if (isScopePressed)
                {
                    isScopePressed = false;
                    return true;
                }

                return false;
            }
        }

        public bool JumpPressed
        {
            get
            {
                if (isJumpPressed)
                {
                    isJumpPressed = false;
                    return true;
                }

                return false;
            }
        }

        public bool FirePressed
        {
            get { return isFirePressed; }
            set { isFirePressed = value; }
        }

        public bool ReloadPressed
        {
            get
            {
                if (isReloadPressed)
                {
                    isReloadPressed = false;

                    if (isUsedReloadUI)
                    {
                        CancelInvoke("DisableReloadUI");
                        DisableReloadUI();
                    }

                    return true;
                }

                return false;
            }
        }

        public Vector3 PositionWhereToFire
        {
            get { return fireShootAt; }
        }

        public string HitObjectName
        {
            get { return hitObjectName; }
        }

        public string HitObjectTag
        {
            get { return hitObjectTag; }
        }
        // Player's variables ⬆


        // Car variables ⬇
        public float VerticalCarMovementVar
        {
            get { return verticalCarMovementVar; }
        }

        public float HorizontalCarMovementVar
        {
            get { return horizontalCarMovementVar; }
        }

        public bool IsBrakePressed
        {
            get { return isBrakePressed; }
        }
        // Car variables ⬆

    }
}