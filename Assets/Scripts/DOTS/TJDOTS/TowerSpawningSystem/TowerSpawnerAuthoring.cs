using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace TJ.DOTS
{
    public class TowerSpawnerAuthoring : MonoBehaviour
    {
        public GameObject TowerPrefab;
        class Baker : Baker<TowerSpawnerAuthoring>
        {
            public override void Bake(TowerSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TowerSpawningProperties {
                    TowerPrefab = GetEntity(authoring.TowerPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}
