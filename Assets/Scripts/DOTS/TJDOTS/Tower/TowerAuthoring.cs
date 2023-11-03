using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{
    public class TowerAuthoring : MonoBehaviour
    {
       public GameObject TowerPrefab;
        class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TowerSpawningProperties {
                    TowerPrefab = GetEntity(authoring.TowerPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
    public struct TowerEntity : IComponentData
    {

    }
}
