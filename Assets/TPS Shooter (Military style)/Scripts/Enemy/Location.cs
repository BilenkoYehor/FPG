using System.Collections.Generic;
using UnityEngine;

namespace TPSShooter
{

    /// <summary>
    /// This class contains information about Location (size in world points, list of enemies)
    /// </summary>

    public class Location
    {

        readonly List<EnemyData> enemies = new List<EnemyData>();
        Vector2 minCoordinates;
        Vector2 maxCoordinates;

        bool hasGeneratedEnemies;

        public Location(Transform locationObj)
        {
            minCoordinates.x = locationObj.position.x - locationObj.localScale.x / 2;
            minCoordinates.y = locationObj.position.z - locationObj.localScale.z / 2;
            maxCoordinates.x = locationObj.position.x + locationObj.localScale.x / 2;
            maxCoordinates.y = locationObj.position.z + locationObj.localScale.z / 2;
        }

        public bool AtThisLocation(float x, float z)
        {
            return (x >= minCoordinates.x && x <= maxCoordinates.x) && (z >= minCoordinates.y && z <= maxCoordinates.y);
        }

        public bool ContainsEnemies()
        {
            return enemies.Count != 0;
        }

        public List<EnemyData> EnemyList
        {
            get
            {
                return enemies;
            }
        }

        public bool HasGeneratedEnemies
        {
            set
            {
                hasGeneratedEnemies = value;
            }
            get
            {
                return hasGeneratedEnemies;
            }
        }

        public void AddEnemy(EnemyData enemy)
        {
            enemies.Add(enemy);
        }

    }
}