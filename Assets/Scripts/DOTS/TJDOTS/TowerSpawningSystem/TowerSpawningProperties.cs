using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
public struct TowerSpawningProperties : IComponentData
{
    public Entity TowerPrefab;
}
}