using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{

    [Header("Bullet settings")]
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    public float bulletLifeTime;

    [Header("Gun settings")]
    public float shootFrequency;

    [Header("Sounds")]
    public AudioSource fireSound;

    [Header("Fire particle")]
    public ParticleSystem fireParticleSystem;

    bool canShoot = true;
    public bool CanShoot
    {
        get { return canShoot; }
    }

    // Shoot
    public void Fire(Vector3 positionWhereToFire)
    {
        if (canShoot)
        {
            // makes shooting unavailable for shootFrequency time
            canShoot = false;
            Invoke("CanShootTrue", shootFrequency);

            // Sound
            fireSound.Stop();
            fireSound.Play();

            // Particle
            fireParticleSystem.Stop();
            fireParticleSystem.Play();

            // Bullet position
            bulletPosition.LookAt(positionWhereToFire);

            // Instantiates bullet
            GameObject myBullet = Instantiate(bulletPrefab);
            myBullet.transform.position = bulletPosition.transform.position;
            myBullet.transform.eulerAngles = bulletPosition.transform.eulerAngles;

            myBullet.GetComponent<Bullet>().MasterOfBullet = transform;
            Destroy(myBullet, bulletLifeTime);
        }
    }

    // Makes shooting available
    void CanShootTrue()
    {
        canShoot = true;
    }

}