using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Bullet : MonoBehaviour
{

    [Header("Bullets Settings")]
    // Start Speed
    public float startSpeed;
    // Damage
    public float damage;

    [Header("Bullet impacts")]
    public bool withBulletHole;
    public GameObject bulletHole;
    public float bulletHoleLifetime = 5f;

    Rigidbody rb;

    float shootTime;
    Vector3 startingAngle, startingPosition;

    Transform masterOfBullet;
    public Transform MasterOfBullet
    {
        get { return masterOfBullet; }
        set { masterOfBullet = value; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        GetComponent<CapsuleCollider>().isTrigger = true;

        shootTime = Time.time;

        startingPosition = transform.position;
        startingAngle = transform.forward;
    }

    void Update()
    {

        if (rb.isKinematic == false)
            return;
        float time = Time.time - shootTime;
        Vector3 newPos = (startingAngle * startSpeed * time) + startingPosition;
        transform.LookAt(newPos);

        float speed = Vector3.Distance(transform.position, newPos);
        float force = (speed / Time.deltaTime) * (speed / Time.deltaTime) * rb.mass / 2f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, startingAngle, out hit, speed))
        {
            transform.position = hit.point;
            rb.isKinematic = false;
            rb.AddForceAtPosition(transform.forward * force, hit.point);
            // Influence on hit Object
            if (hit.transform.GetComponent<Rigidbody>())
            {
                hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * force, hit.point);
            }

            // Visual impact
            if (withBulletHole)
            {
                if (!hit.transform.tag.Contains("Player") && !hit.transform.tag.Contains("Bullet") && !LayerMask.LayerToName(hit.transform.gameObject.layer).Contains("Enemy"))
                {
                    GameObject hole = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up * 1.05f, hit.normal));
                    Destroy(hole, bulletHoleLifetime);
                }
            }
        }
        else
        {
            transform.position = newPos;
        }
    }
}