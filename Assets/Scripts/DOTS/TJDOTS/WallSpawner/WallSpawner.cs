using Unity.Entities;
using Unity.Mathematics;

public struct WallSpawner : IComponentData
{
    public Entity Prefab;
    public float3 SpawnPosition;
    // public float NextSpawnTime;
    // public float SpawnRate;
    public int SpawnCount;
    public int MaxSpawnCount;
}
