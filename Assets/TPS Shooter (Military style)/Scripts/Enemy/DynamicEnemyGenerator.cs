using System.Collections.Generic;
using UnityEngine;

namespace TPSShooter
{

    public class DynamicEnemyGenerator : MonoBehaviour
    {

        [Header("Enemies Prefabs")]
        public GameObject[] enemiesPrefabs;

        [Header("Generation Settings")]
        public float minTimeGeneration = 5f;
        public float maxTimeGeneration = 10f;

        public float maxDistanceToPlayer = 20f;

        public int maxNpcCount = 3;

        readonly List<GameObject> enemiesList = new List<GameObject>();

        bool canGenerate;

        GameManager gameManager;
        GameObject player;

        void Start()
        {
            player = GameObject.FindWithTag(Tags.Player);
            if (!player)
                Debug.LogError("DynamicEnemyGenerator: no player found in the game.");
            Invoke("GenerateEnemies", 5f);

            if (!GameObject.FindWithTag(Tags.GameManager))
                Debug.LogError("DynamicEnemyGenerator: no GameManager was found in the game.");
            gameManager = GameObject.FindWithTag(Tags.GameManager).GetComponent<GameManager>();
        }

        void Update()
        {
            // delete null references in the lise (for example, enemy was destroyed and now it has null reference)
            enemiesList.Remove(null);

            // If there are no generated enemies, generate enemies
            if (enemiesList.Count == 0 && canGenerate)
            {
                Invoke("GenerateEnemies", Random.Range(minTimeGeneration, maxTimeGeneration));
                canGenerate = false;
            }
        }

        /// <summary>
        /// Generates enemies.
        /// </summary>
        void GenerateEnemies()
        {
            if (!gameManager.IsGameStopped)
            {
                // random count of enemies
                byte count = (byte)Random.Range(1, maxNpcCount + 1);
                // selects type that will be generated
                byte npcType = (byte)Random.Range(0, enemiesPrefabs.Length);

                // generate enemies
                for (int i = 0; i < count; i++)
                {
                    Vector3 r = new Vector3(Random.Range(player.transform.right.x, -player.transform.right.x),
                                                player.transform.right.y,
                                                Random.Range(player.transform.right.z, -player.transform.right.z));
                    Vector3 pos = player.transform.position + (r * maxDistanceToPlayer);

                    if (!Physics.Linecast(player.transform.position, pos))
                    {
                        GameObject obj = Instantiate(enemiesPrefabs[npcType]);

                        obj.transform.position = pos;

                        // add to the list
                        enemiesList.Add(obj);
                    }
                }
            }
            canGenerate = true;
        }

    }
}