using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(RadarableObject))]
[RequireComponent(typeof(CharacterController))]
public class EnemyLow : MonoBehaviour {

    public EnemyAnimationParameters animatorParameters;

    public EnemyVisionSettings visionSettings;

    public EnemyPatrollingSettings patrollingSettings;

    public EnemyWeaponSettings weaponSettings;

    public EnemyDeathSettings deathSettings;

    public EnemyAIBehaviour aiBehaviour;

    [Range(1,5)]
    public int rayFrameInterval;

    [Space]
    [Tooltip("Distance between player and enemy, when enemy can shoot.")]
    public float shootRadius = 40f;

    // Blood effect
    [Space]
    public ParticleSystem bloodEffect;

    Player playerScript;
    Transform player;

    EnemyState state = EnemyState.Patrol;

    CharacterController characterController;
    Animator animator;

    // If Enemy is Alive
    bool isAlive = true;

    float HP = 100f;

    // Enemy's part to calculate damage from a hit player's bullet
    float oneBodyPart;

    // animator parameters
    float forward;
    float strafe;

    void Start()
    {
        // Sets to kinematic in order to Rigidbodies do not interact with CharacterController
        if (deathSettings.isRagdolled)
        {
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody b in bodies)
                b.isKinematic = true;
        }

        // Gets the animator, characterController and capsuleCollider
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        //  playerScript = GameObject.fin
        player = GameObject.FindWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
        //playerScript.

        // Sets oneBodyPart value
        oneBodyPart = characterController.height / 8f;

        if (weaponSettings.timeBeforeShot > weaponSettings.weapon.shootFrequency || weaponSettings.timeBeforeShot < 0)
            Debug.LogError("EnemyBehaviour: WeaponSettings.timeBeforeShot has to be more or equal 0 and less than EnemyWeapon.shootFrequency");

        if (patrollingSettings.waypoints.Length == 1 || patrollingSettings.waypoints.Length == 2)
            Debug.LogError("EnemyBehaviour: PatrollingSettings amount of waypoints has to be more than 2 or equal 0");

        if (!gameObject.tag.Contains("Enemy"))
            Debug.LogError("EnemyBahaviour: GameObject doesn't have tag that starts with Enemy...");
        if (!LayerMask.LayerToName(gameObject.layer).Equals("Enemy"))
            Debug.LogError("EnemyBahaviour: GameObject has to be layered as Enemy.");
    }

    // The position where enemy see the player at the moment or for the last time
    Vector3 previousPlayerPosition;

    void Update()
    {
        if (isAlive && playerScript.IsAlive)
        {
            if (Time.frameCount % rayFrameInterval == 0)
            {
                PlayerRayNotice();
            }

            // Detection
            if ((PlayerNoiseNotice() || PlayerVisionNotice()) && rayDetection)
            {
                // stores position, where the enemy saw the player for the last time
                previousPlayerPosition = player.transform.position;

                // Shoot
                if (DistaceToPlayer() < shootRadius)
                {
                    state = EnemyState.Attack;
                }
                // Move
                else
                {
                    state = EnemyState.Search;
                }
            }
            // After enemy's attack, enemy goes to the position where he saw the player for the last time
            else if (state == EnemyState.Attack)
            {
                state = EnemyState.Search;
            }


            switch (state)
            {
                case EnemyState.Patrol:
                    Patrol();
                    break;

                case EnemyState.Attack:
                    // movement while shooting
                    switch (aiBehaviour.attackMotion)
                    {
                        case AttackMotion.None:
                            forward = 0;
                            strafe = 0;
                            break;

                        case AttackMotion.Strafe:
                            ShootingStrafe();
                            break;
                    }
                    // shooting
                    ShootToPlayer();
                    break;

                // finding the player
                case EnemyState.Search:
                    switch (aiBehaviour.searchSettings)
                    {
                        case SearchSettings.None:
                            forward = 0;
                            strafe = 0;
                            // nothing
                            break;
                        case SearchSettings.Search:
                            MoveToPosition(previousPlayerPosition);
                            break;
                    }
                    break;
            }

            // updates parametres of animator
            animator.SetFloat(animatorParameters.forwardHash, forward);
            animator.SetFloat(animatorParameters.strafeHash, strafe);

            // Udpates gravity
            UpdateEnemyGravity();
        }
    }

    float damage;

    void OnTriggerEnter(Collider other)
    {
        // Collides with player's bullet
        if (isAlive)
        {
            if (other.tag.Equals("PlayerBullet"))
            {
                // blood effect
                bloodEffect.Play();

                // damage
                damage = (other.gameObject.transform.position.y - transform.position.y) / oneBodyPart
                        * other.gameObject.GetComponent<Bullet>().damage;
                HP -= damage;

                // checks if the enemy can be alive
                if (HP <= 0)
                {
                    EnemyDie();
                }

                // destroy bullter
                Destroy(other.gameObject);
            }
        }
    }

    /*
     * 
     /////////////////////////////////////////////////////////////////////////////////////////////////////////// Enemy behaviour methods ⬇︎
     * 
     */

    enum EnemyState
    {
        Patrol, Search, Attack
    }

    public void EnemyDie()
    {
        if (isAlive)
        {
            isAlive = false;

            // disable CharacterController and CapsuleCollider
            characterController.enabled = false;

            if (deathSettings.isRagdolled)
            {
                animator.enabled = false;

                Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody b in bodies)
                    b.isKinematic = false;
            }
            else
            {
                // Play animation
                animator.SetTrigger(animatorParameters.dieHash);
            }

            // unattach gameObjects
            for (int i = 0; i < deathSettings.items.Length; i++)
            {
                deathSettings.items[i].parent = null;
                deathSettings.items[i].GetComponent<Rigidbody>().isKinematic = false;
                Destroy(deathSettings.items[i].gameObject, deathSettings.enemyDieTime);
            }

            GetComponent<RadarableObject>().DestroyDot();

            // Destroy enemy
            Destroy(gameObject, deathSettings.enemyDieTime);

            // Destroy blood effect
            Destroy(bloodEffect.gameObject, bloodEffect.main.duration);
        }
    }

    /// <summary>
    /// Patrolling.
    /// </summary>
    int waypointIndex = 0;
    float currentWaitTime;

    void Patrol()
    {
        if (patrollingSettings.waypoints.Length == 0)
            return;

        strafe = 0;

     //   navmeshAgent.SetDestination(patrollingSettings.waypoints[waypointIndex].destination.position);
       // LookAtPosition(navmeshAgent.steeringTarget);

        if (Vector3.Distance(transform.position, patrollingSettings.waypoints[waypointIndex].destination.position) <= 1.5f)
        {
            forward = LerpSpeed(forward, 0, 5);
            currentWaitTime -= Time.deltaTime;

            if (currentWaitTime <= 0)
            {
                waypointIndex = (waypointIndex + 1) % patrollingSettings.waypoints.Length;
            }
        }
        else
        {
            forward = LerpSpeed(forward, 1, 5);
            currentWaitTime = patrollingSettings.waypoints[waypointIndex].waitTime;
        }
    }

    /// <summary>
    /// Movement methods while enemy is shooting.
    /// </summary>
    void ShootingStrafe()
    {
        timeLeft += Time.deltaTime;

        if (timeLeft > shootingStrafeTime)
        {
            shootingStrafeTime = Random.Range(0.5f, 1.2f);
            timeLeft = 0;

            canMoveLeft = !canMoveLeft;
        }

        forward = 0;
        if (canMoveLeft)
            strafe = 1;
        else
            strafe = -1;
    }
    bool canMoveLeft = true;
    float shootingStrafeTime;
    float timeLeft;

    /// <summary>
    /// Shoots player.
    /// </summary>
    void ShootToPlayer()
    {
        // Looks at player for period of time (weaponBehaviour.shootFrequency - aimTime) and stores position of the player
        if (canLookAtPlayer)
        {
            previousFirePlayerPosition = player.transform.position;
        }

        // Shoot
        if (canShoot && weaponSettings.weapon.CanShoot)
        {
            Invoke("CantLookAtPlayer", weaponSettings.weapon.shootFrequency - weaponSettings.timeBeforeShot);

            canLookAtPlayer = true;

            weaponSettings.weapon.Fire(previousFirePlayerPosition + new Vector3(0, 1.5f, 0));
        }

        LookAtPosition(player.transform.position);
    }

    Vector3 previousFirePlayerPosition;

    void CantLookAtPlayer()
    {
        canLookAtPlayer = false;

        Invoke("CanShootAvailable", weaponSettings.timeBeforeShot);
    }

    void CanShootAvailable()
    {
        canShoot = true;
    }

    bool canShoot = true;

    // this variable depends on aimTime variable
    bool canLookAtPlayer = true;

    /// <summary>
    /// Moves to player.
    /// </summary>
    void MoveToPosition(Vector3 pos)
    {
        strafe = 0;


        if (Vector3.Distance(transform.position, previousPlayerPosition) < 0.3f)
        {
            forward = LerpSpeed(forward, 0, 5);
        }
        else
        {
            LookAtPosition(previousPlayerPosition);
            forward = LerpSpeed(forward, 1, 5);
        }
    }

    float LerpSpeed(float curSpeed, float destSpeed, float time)
    {
        return Mathf.Lerp(curSpeed, destSpeed, Time.deltaTime * time);
    }

    /// <summary>
    /// Used when the enemy uses pathfinding
    /// </summary>
    void LookAtPosition(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0;
        lookRot.z = 0;

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Updates the enemy gravity.
    /// </summary>
    float vSpeed;
    Vector3 moveDirection;
    float gravity = 9.8f;

    void UpdateEnemyGravity()
    {
        if (characterController.isGrounded)
        {
            vSpeed = -1;
        }

        else
        {
            vSpeed -= gravity * Time.deltaTime;
        }

        moveDirection.y = vSpeed - Mathf.Min(0, characterController.velocity.y);
        characterController.Move(moveDirection * Time.deltaTime);
    }

    /*
     * 
     /////////////////////////////////////////////////////////////////////////////////////////////////////////// NPC behaviour methods ⬆︎
     * 
     */

    /*
     * 
     ********************************************************************************************************** Player detection Methods ⬇︎
     * 
     */

    /// <summary>
    /// If ray notice player.
    /// </summary>
    bool rayDetection;
    void PlayerRayNotice()
    {
        RaycastHit hit;

        visionSettings.visionPosition.LookAt(player.position + new Vector3(0, 1f, 0));

        Ray ray = new Ray(visionSettings.visionPosition.position, visionSettings.visionPosition.forward);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, visionSettings.visionLayers))
        {
            if (hit.collider.gameObject.tag == player.tag)
                rayDetection = true;
            else
                rayDetection = false;
        }
        else
        {
            rayDetection = false;
        }
    }

    /// <summary>
    /// Limit of min player's noise that enemy can hear
    /// </summary>
    bool PlayerNoiseNotice()
    {
        return Vector3.Distance(player.position, transform.position) < 7;
        //  return playerBehaviour.Noise > DistaceToPlayer();
    }

    float DistaceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    /*
     * @Param maxAngle, max visionAngle
     * 
     * Return if an Enemy sees Player that is in front of the Enemy 
     */

    float fieldOfView = 80f;

    bool PlayerVisionNotice()
    {
        Vector3 targetDir = player.transform.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);
        return Mathf.Abs(angle) <= fieldOfView;
    }

    /*
     * 
     ********************************************************************************************************** Player detection Methods ⬆︎
     * 
     */

    /// <summary>
    /// Classes for inspector
    /// </summary>

    [System.Serializable]
    public class EnemyAnimationParameters
    {
        // forward
        public string forwardHash = "Forward";
        // strafe
        public string strafeHash = "Strafe";
        // death
        public string dieHash = "Die";
    }

    [System.Serializable]
    public class EnemyVisionSettings
    {
        public Transform visionPosition;
        public LayerMask visionLayers;
    }

    [System.Serializable]
    public class EnemyPatrollingSettings
    {
        public WaypointBase[] waypoints;
    }

    [System.Serializable]
    public class WaypointBase
    {
        public Transform destination;
        // how much time the enemy will be at this position before going on another position
        public float waitTime;
    }

    [System.Serializable]
    public class EnemyWeaponSettings
    {
        public EnemyWeapon weapon;
        [Tooltip("Varible has to be less than WeaponBahaviour.ShootFrequency. This variable show how much time enemy will not be looking at the player before shooting at him")]
        public float timeBeforeShot;
    }

    [System.Serializable]
    public class EnemyDeathSettings
    {
        public bool isRagdolled;
        public float enemyDieTime = 20f;
        [Header("Items that will be unattached when enemy dies")]
        public Transform[] items;
    }

    [System.Serializable]
    public class EnemyAIBehaviour
    {
        public AttackMotion attackMotion;
        public SearchSettings searchSettings;
    }

    [System.Serializable]
    public enum AttackMotion
    {
        None, Strafe
    }

    [System.Serializable]
    public enum SearchSettings
    {
        None, Search
    }

}