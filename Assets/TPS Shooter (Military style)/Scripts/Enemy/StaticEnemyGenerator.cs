using UnityEngine;
using System.Collections.Generic;

namespace TPSShooter
{

    /// <summary>
    /// This class generates enemies at current positions when the player is at the same location.
    /// </summary>
    /// 
    /// <remarks>
    /// Child empty GameObjects must contain Tag.Location.
    /// These empty GameObjects(with Tag.Location) must contain Empty GameObjects with such tags as Enemys tags.
    /// </remarks>

    public class StaticEnemyGenerator : MonoBehaviour
    {

        [Header("Enemies Prefabs")]
        public GameObject[] enemies;

        // Tags of the enemiesPrefabs
        string[] enemiesTags;

        // Locations in the game
        List<Location> locationList = new List<Location>();

        // Player
        GameObject player;

        void Start()
        {
            // fill enemies tags
            enemiesTags = new string[enemies.Length];
            for (int i = 0; i < enemiesTags.Length; i++)
            {
                enemiesTags[i] = enemies[i].tag;
            }

            // finds player
            player = GameObject.FindWithTag(Tags.Player);
            if (!player)
                Debug.LogError("StaticEnemyGenerator: no player found in the game.");

            IntializeLocationsAndEnemies();
        }

        void IntializeLocationsAndEnemies()
        {
            foreach (Transform childLocation in transform)
            {
                if (childLocation.tag.Equals(Tags.Location))
                {
                    // finds location by tag
                    Location newLocation = new Location(childLocation);
                    foreach (Transform locationChildEnemy in childLocation)
                    {
                        // initialize enemy
                        for (int i = 0; i < enemiesTags.Length; i++)
                        {
                            if (locationChildEnemy.tag.Equals(enemiesTags[i]))
                                newLocation.AddEnemy(new EnemyData(locationChildEnemy, enemies[i]));
                        }
                    }
                    // if this location contains enemies, add this location to locationList
                    if (newLocation.ContainsEnemies())
                    {
                        locationList.Add(newLocation);
                    }
                }
            }
        }

        void Update()
        {
            CheckPlayerPositionAtLocations();
        }

        Location currentLocation;

        /// <summary>
        /// Checks the player position and if the player in location, generate enemies
        /// </summary>
        void CheckPlayerPositionAtLocations()
        {
            bool findLocation = false;

            foreach (Location l in locationList)
            {
                print(l.ToString());
                // finds location where the player is and generate them
                if (l.AtThisLocation(player.transform.position.x, player.transform.position.z))
                {
                    findLocation = true;
                    currentLocation = l;
                    if (!l.HasGeneratedEnemies)
                    {
                        l.HasGeneratedEnemies = true;
                        GenerateEnemies(l);
                    }
                }
            }
            // if the player is not in any location, destroy enemies at the last location
            if (!findLocation && currentLocation != null)
            {
                if (currentLocation.HasGeneratedEnemies)
                {
                    currentLocation.HasGeneratedEnemies = false;
                    DestroyEnemiesAtLocation(currentLocation);
                }
            }
        }

        /// <summary>
        /// Generates the enemies at location.
        /// </summary>
        void GenerateEnemies(Location l)
        {
            foreach (EnemyData npc in l.EnemyList)
            {
                npc.CreatedObj = Instantiate(npc.EnemyGameObject);
                npc.CreatedObj.transform.position = npc.Transform.position;
                npc.CreatedObj.transform.rotation = npc.Transform.rotation;
            }
        }

        /// <summary>
        /// Destroies the enemies at location.
        /// </summary>
        void DestroyEnemiesAtLocation(Location l)
        {
            foreach (EnemyData npc in l.EnemyList)
            {
                Destroy(npc.CreatedObj);
            }
        }

    }
}