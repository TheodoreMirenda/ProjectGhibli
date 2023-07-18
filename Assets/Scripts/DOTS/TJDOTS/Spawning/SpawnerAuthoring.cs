using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using TJ.DOTS.Execute;

namespace TJ.DOTS
{
    // An authoring component is just a normal MonoBehavior that has a Baker<T> class.
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject GoblinPrefab;
        public int GoblinCount;
        // public float SafeZoneRadius;
        public float2 FieldDimensions;
        public uint RandomSeed;

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                // AddComponent<Prefabs>(entity);
                // AddComponent(entity, new Spawner {
                //     Prefab = GetEntity(authoring.GoblinPrefab, TransformUsageFlags.Dynamic)
                // });
                // AddComponent(entity, new GoblinSpawningProperties
                // {
                //     FieldDimensions = authoring.FieldDimensions,
                //     NumberOfGoblinsToSpawn = authoring.NumberTombstonesToSpawn,
                //     GoblinPrefab = GetEntity(authoring.GoblinPrefab, TransformUsageFlags.Dynamic)
                // });
                AddComponent(entity, new GoblinSpawningRandom
                {
                    Value = Random.CreateFromIndex(authoring.RandomSeed)
                });
                // AddComponent<ZombieSpawnPoints>(entity);
                AddComponent(entity, new Config
                {
                    GoblinPrefab = GetEntity(authoring.GoblinPrefab, TransformUsageFlags.Dynamic),
                    GoblinCount = authoring.GoblinCount
                    // SafeZoneRadius = authoring.SafeZoneRadius
                });
            }
        }
    }
    public struct Config : IComponentData
    {
        public Entity GoblinPrefab;
        public int GoblinCount;
        // public float SafeZoneRadius;   // Used in a later step.
    }

    // struct Spawner : IComponentData
    // {
    //     public Entity Prefab;
    // }
}
