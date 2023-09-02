using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TJ.Utilities;

namespace Gobillions
{
public class GoblinSpawner : MonoBehaviour
{
    [SerializeField] private  Wave[] waves;
    private List<Collider> spawnZones;

    //cached
    Enemy enemyToSpawn;
    int enemiesRemainingToSpawn, enemiesRemainingAlive, currentWaveNumber = 0;
    float nextSpawnTime;
    private Wave currentWave;

    // public bool spawnClose;
    // [SerializeField] public float minDistanceFromPlayerToSpawn, maxDistanceFromPlayerToSpawn;

    private void Awake() {
        spawnZones = new List<Collider>(transform.GetComponentsInChildren<Collider>());
        NextWave();
    }
    public string FormatTime( float time )
    {
        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds );
    }
    public void Update(){

        if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            enemyToSpawn = currentWave.enemies[enemiesRemainingToSpawn];

            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Vector3 spawnPoint = RandomPointInBounds(spawnZones[Random.Range(0,spawnZones.Count)].bounds);

            Enemy spawnedEnemy = Instantiate(enemyToSpawn, spawnPoint, Quaternion.identity) as Enemy;
            // spawnedEnemy.OnDeath += OnEnemyDeath;

            if(enemiesRemainingToSpawn==0)
                NextWave();
        }
    }
    public void RespawnEnemy(Enemy spawnedEnemy)
    {
        if(spawnZones.Count==0)
        {
            Debug.Log($"No spawn zones found for {spawnedEnemy.name}");
            return;
        }

        Vector3 spawnPoint = RandomPointInBounds(spawnZones[Random.Range(0,spawnZones.Count)].bounds);

        spawnedEnemy.gameObject.transform.position = spawnPoint;
    }
    public void NextWave()
    {
        if(currentWaveNumber == waves.Length){
            // Debug.Log($"All waves complete!");
            return;
        }

        currentWave = waves[currentWaveNumber];
        currentWave.CreateWave();

        enemiesRemainingToSpawn = currentWave.enemies.Count;
        enemiesRemainingAlive = enemiesRemainingToSpawn;
        // Debug.Log($"Wave {currentWave} started");
        currentWaveNumber++;
    }
    [System.Serializable] public class Wave{
        public float timeBetweenSpawns;
        public List<SubWave> subWaves;
        [HideInInspector] public List<Enemy> enemies = new List<Enemy>();
        public void CreateWave()
        {
            foreach(SubWave subWave in subWaves) {
                for(int i = 0; i < subWave.enemyCount; i++)
                    enemies.Add(subWave.enemy);
            }
            ListExtensions.Shuffle(enemies);
        }
    }
    [System.Serializable] public class SubWave{
        public int enemyCount;
        public Enemy enemy;
    }
    
    public static Vector3 RandomPointInBounds(Bounds bounds) {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
}