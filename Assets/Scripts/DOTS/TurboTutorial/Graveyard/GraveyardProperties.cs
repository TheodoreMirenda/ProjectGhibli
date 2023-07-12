using Unity.Entities;
using Unity.Mathematics;

namespace TMG.Zombies
{
    //RefRO
    public struct GraveyardProperties : IComponentData
    {
        public float2 FieldDimensions;
        public int NumberTombstonesToSpawn;
        public Entity TombstonePrefab;
        public Entity ZombiePrefab;
        public float ZombieSpawnRate;
    }
    //RefRW
    //we want this seprate so that we can write to it in the system
    public struct ZombieSpawnTimer : IComponentData
    {
        public float Value;
    }
}