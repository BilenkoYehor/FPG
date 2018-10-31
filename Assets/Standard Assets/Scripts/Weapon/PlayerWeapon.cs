using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

    [Header("- Aiming positions -")]
    public Vector3 localPos;
    public Vector3 localRot;
    public Vector3 localAimPos;
    public Vector3 localReloadPos;
    public Vector3 localRunningPos;

    [Header("- Scope settings -")]
    public float fieldOfView = 50;
    public float followVelocitiy = 10f;

    [Header("- Weapon settings -")]
    [Range(0, 0.1f)]
    public float spread = 0.05f;
    public float shootFrequency = 0.12f;
    public int magCapacity= 30;
    public int bulletsInMag =30;
    public int availableBullets = 30;

    [Header("- Fire settings -")]
    public GameObject bulletPrefab;
    public Transform bulletPos;
    public float bulletLifeTime = 0.8f;
    public ParticleSystem fireParticleSystem;

    [Header("- Sounds -")]
    AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip idleSound;
    public AudioClip reloadSound;

    [Header("- Player -")]
    public Player player;

    Animator animator;

    bool isAiming;
    public bool IsAiming
    {
        set 
        { 
            isAiming = value;

            if (isAiming)
                animator.SetBool("IsAiming", true);
            else
                animator.SetBool("IsAiming", false);
        }
    }

    bool isReloading;
    public bool IsReloading
    {
        get { return isReloading; }
    }

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
	}

    // Update is called once per frame
    void LateUpdate()
    {
        if (isReloading)
        {
            PositionWeapon(localReloadPos);
        }
        else if (isAiming)
        {
            PositionWeapon(localAimPos);
        }
        else if (isRunning)
        {
            PositionWeapon(localRunningPos);
        }
        else
        {
            PositionWeapon(localPos);
        }
    }

    bool canShoot = true;
    public void Fire(Vector3 hitPos)
    {
        if (!isReloading)
        {
            if (canShoot)
            {
                if (bulletsInMag > 0)
                {
                    animator.SetTrigger("Shot");
                    
                    // makes shooting unavailable for shootFrequency time
                    canShoot = false;
                    Invoke("CanShootNow", shootFrequency);

                    hitPos.x += Random.Range(-spread, spread);
                    hitPos.y += Random.Range(-spread, spread);
                    hitPos.z += Random.Range(-spread, spread);
                    
                    // Decrement bullet count
                    bulletsInMag--;

                    // Sound
                    audioSource.clip = fireSound;
                    audioSource.Play();

                    // Particle
                    fireParticleSystem.Stop();
                    fireParticleSystem.Play();

                    // Bullet position
                    if (hitPos != Vector3.zero)
                    {
                        bulletPos.LookAt(hitPos);
                    }
                    else
                    {
                        bulletPos.eulerAngles = Vector3.up;
                    }

                    // Instantiates bullet
                    GameObject myBullet = Instantiate(bulletPrefab);
                    myBullet.transform.position = bulletPos.transform.position;
                    myBullet.transform.eulerAngles = bulletPos.transform.eulerAngles;
                    Destroy(myBullet, bulletLifeTime);
                }
                else
                {
                    if (availableBullets > 0)
                        player.Reload();
                    else
                    {
                        // makes shooting unavailable for shootFrequency time
                        canShoot = false;
                        Invoke("CanShootNow", shootFrequency * 4);
                        audioSource.clip = idleSound;
                        audioSource.Play();
                    }
                }
            }
        }
    }

    // Makes shooting available
    void CanShootNow()
    {
        canShoot = true;
    }

    bool isRunning;
    public void RunAnimator(bool isRun)
    {
        animator.SetBool("IsWalking", isRun);
      //  isRunning = isRun;
    }

    public void WalkAnimator(bool isWalk)
    {
        animator.SetBool("IsWalking", isWalk);
    }

    public void Reload()
    {
        animator.SetBool("IsReloading", true);

        isReloading = true;

        audioSource.clip = reloadSound;
        audioSource.Play();
    }

    int addBulletsCount;
    public void ReloadFinished()
    {
        animator.SetBool("IsReloading", false);

        isReloading = false;

        addBulletsCount = Mathf.Min(magCapacity - bulletsInMag, availableBullets);
        bulletsInMag += addBulletsCount;
        availableBullets -= addBulletsCount;

        player.UpdateBulletsCount();
    }

    float currentVelocity;
    void PositionWeapon(Vector3 newPos)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, Time.deltaTime * followVelocitiy);
    }

    public void OnChanged()
    {
        isReloading = false;
        isAiming = false;
        canShoot = true;
    }

}