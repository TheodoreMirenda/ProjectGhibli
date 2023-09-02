using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace TJ.DOTS
{
    // An authoring component is just a normal MonoBehavior that has a Baker<T> class.
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject GoblinPrefab;
        public int GoblinCount;
        public float SafeZoneRadius;
        public float2 FieldDimensions;
        public uint RandomSeed;

        // In baking, this Baker will run once for every SpawnerAuthoring instance in a subscene.
        // (Note that nesting an authoring component's Baker class inside the authoring MonoBehaviour class
        // is simply an optional matter of style.)
        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GoblinRandom {
                    RandomValue = Random.CreateFromIndex(authoring.RandomSeed)
                });
                AddComponent(entity, new GoblinSpawningProperties {
                    FieldDimensions = authoring.FieldDimensions,
                    GoblinPrefab = GetEntity(authoring.GoblinPrefab, TransformUsageFlags.Dynamic),
                    NumberOfGoblinsToSpawn = authoring.GoblinCount,
                    SafeZoneRadius = authoring.SafeZoneRadius
                });
            }
        }
    }
}
