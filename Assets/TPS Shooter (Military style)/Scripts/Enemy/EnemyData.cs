using UnityEngine;

namespace TPSShooter
{

    /// <summary>
    /// This class contains enemyPrefab, position where the enemyPrefab will be generated and generated GameObject that can be destroyed.
    /// </summary>

    public class EnemyData
    {

        // Position where enemy will be generated
        Transform transform;

        // Prefab of the enemy
        GameObject enemyPrefab;
        // Created object
        GameObject createdObj;

        public EnemyData(Transform transform, GameObject enemyPrefab)
        {
            this.enemyPrefab = enemyPrefab;
            this.transform = transform;
        }

        public Transform Transform
        {
            get { return transform; }
        }

        public GameObject CreatedObj
        {
            set
            {
                createdObj = value;
            }
            get
            {
                return createdObj;
            }
        }

        public GameObject EnemyGameObject
        {
            get { return enemyPrefab; }
        }

    }
}