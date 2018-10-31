using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour
{
    public FpsCamera fpsCamera;

    [Header("- Weapons -")]
    public GameObject[] weaponObjects;
    public PlayerWeapon firstWeapon;
    public PlayerWeapon secondWeapon;

    [Header("- Scope notifiers -")]
    public GameObject[] scopeNotifiersObjects;
    IScopeNotifier[] scopeNotifiers;

    [Header("- Attached scripts -")]
    public FirstPersonController firstPersonController;

    [Header("- UI -")]
    public Text hpText;
    public Text bulletsText;
    public GameObject[] allUI;
    float HP = 100;

    [Header("- Damage UI -")]
    public GameObject damageImgPrefab;
    public Transform damageImgParent;

    [Header("- Death settings -")]
    public GameObject deathPlayer;
    public GameObject alivePlayer;
    CharacterController characterController;
    Rigidbody rb;

    [Header("- Sounds -")]
    public AudioSource painSound;

    public GameManager gameManager;

    InputController inputController;

    bool isScoping;
    public bool IsAiming
    {
        get { return isScoping; }
    }

    bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
    }

    // Use this for initialization
    void Start()
    {
        inputController = GameObject.FindWithTag("InputController").GetComponent<InputController>();

        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        scopeNotifiers = new IScopeNotifier[scopeNotifiersObjects.Length];
        for (int i = 0; i < scopeNotifiersObjects.Length; i++)
        {
            scopeNotifiers[i] = scopeNotifiersObjects[i].GetComponent<IScopeNotifier>();
        }


        fpsCamera.SetAimingFOV(firstWeapon.fieldOfView);

        secondWeapon.gameObject.SetActive(false);

        SaveLoad.LoadPlayerWeaponPreferences();
        firstWeapon = weaponObjects[SaveLoad.CurrentWeaponIndex].GetComponent<PlayerWeapon>();
        firstWeapon.gameObject.SetActive(true);

        firstWeapon.bulletsInMag = SaveLoad.CurrentBulletsInMag;
        firstWeapon.availableBullets = SaveLoad.CurrentAvailableBullets;


        hpText.text = "100/100";
        UpdateBulletsCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (inputController.IsScopePressed && !GetCurrentWeapon().IsReloading)
            {
                Scope();
            }

            if (inputController.IsReloadPressed && GetCurrentWeapon().availableBullets > 0 && GetCurrentWeapon().bulletsInMag != GetCurrentWeapon().magCapacity)
            {
                Reload();
            }

            if (inputController.IsFirePressed)
            {
                GetCurrentWeapon().Fire(inputController.HitPos);
                UpdateBulletsCount();
            }

            // updates weapon animator idle - walk - run
            if (firstPersonController.IsWalking)
            {
                GetCurrentWeapon().WalkAnimator(true);
                GetCurrentWeapon().RunAnimator(false);
            }
            else if (firstPersonController.IsRunning)
            {
                GetCurrentWeapon().WalkAnimator(false);
                GetCurrentWeapon().RunAnimator(true);
            }
            else
            {
                GetCurrentWeapon().WalkAnimator(false);
                GetCurrentWeapon().RunAnimator(false);
            }

            if (inputController.IsWeaponChanged)
            {
                if (isScoping)
                    Scope();
                ChangeWeapon();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EnemyBullet"))
        {
            HP -= other.GetComponent<Bullet>().damage;

            if (HP < 0)
            {
                Die();
            }
            else
            {
                if (!painSound.isPlaying)
                    painSound.Play();

                hpText.text = string.Format("100/{0}", (int) HP);

                Vector3 radarPos = (other.gameObject.GetComponent<Bullet>().MasterOfBullet.position - transform.position);

                float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - transform.eulerAngles.y;
                radarPos.x = Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
                radarPos.y = Mathf.Sin(deltay * Mathf.Deg2Rad);

                GameObject di = Instantiate(damageImgPrefab, damageImgParent);
                di.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(radarPos.x, radarPos.y) * -Mathf.Rad2Deg));
                Destroy(di, 1f);
            }
        }
    }

    void Die()
    {
        gameManager.FinishGame();
        
        foreach (GameObject uiObj in allUI)
            uiObj.SetActive(false);

        isAlive = false;

        characterController.enabled = false;
        rb.isKinematic = true;

        alivePlayer.SetActive(false);
        deathPlayer.SetActive(true);
    }

    public void Reload()
    {
        if (isScoping)
        {
            Scope();
        }

        GetCurrentWeapon().Reload();
    }

    public void UpdateBulletsCount()
    {
        bulletsText.text = string.Format("{0}/{1}", GetCurrentWeapon().availableBullets, GetCurrentWeapon().bulletsInMag);
    }

    void Scope()
    {
        isScoping = !isScoping;
        GetCurrentWeapon().IsAiming = isScoping;
        fpsCamera.IsAiming = isScoping;

        if (isScoping)
        {
            foreach (IScopeNotifier notifier in scopeNotifiers)
                notifier.ScopeOn();
        }
        else
        {
            foreach (IScopeNotifier notifier in scopeNotifiers)
                notifier.ScopeOff();
        }
    }

    public bool CanRun()
    {
        return !GetCurrentWeapon().IsReloading && !isScoping;
    }

    void ChangeWeapon()
    {
        GetCurrentWeapon().OnChanged();

        currentWeapon = (currentWeapon + 1) % 2;

        if (currentWeapon == 0)
        {
            firstWeapon.gameObject.SetActive(true);
            secondWeapon.gameObject.SetActive(false);
        }
        else
        {
            firstWeapon.gameObject.SetActive(false);
            secondWeapon.gameObject.SetActive(true);
        }

        UpdateBulletsCount();
    }

    int currentWeapon = 0;
    public PlayerWeapon GetCurrentWeapon()
    {
        if (currentWeapon == 0)
            return firstWeapon;
        return secondWeapon;
    }

    public void OnGameStopped()
    {
        foreach (GameObject uiObj in allUI)
            uiObj.SetActive(false);
    }

    public void OnGameResumed()
    {
        foreach (GameObject uiObj in allUI)
            uiObj.SetActive(true);
    }

    public void OnGameFinished()
    {
        
    }
}