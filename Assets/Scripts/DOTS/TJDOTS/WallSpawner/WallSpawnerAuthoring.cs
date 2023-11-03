using UnityEngine;
using Unity.Entities;

class WallSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public int spawnAmount;
}

class SpawnerBaker : Baker<WallSpawnerAuthoring>
{
    public override void Bake(WallSpawnerAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent(entity, new WallSpawner
        {
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            SpawnPosition = authoring.transform.position,
            SpawnCount = authoring.spawnAmount,
            MaxSpawnCount = authoring.spawnAmount
        });
    }
}
