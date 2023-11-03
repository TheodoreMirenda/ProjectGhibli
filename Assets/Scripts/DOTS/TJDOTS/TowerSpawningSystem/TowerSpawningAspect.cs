using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
public readonly partial struct TowerSpawningAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRO<LocalTransform> _transformAspect;
    private LocalTransform Transform => _transformAspect.ValueRO;
    private readonly RefRO<TowerSpawningProperties> _towerSpawningProperties;
    public Entity TowerPrefab => _towerSpawningProperties.ValueRO.TowerPrefab;

    public LocalTransform GetRandomSpawningSpot() {
        return new LocalTransform
        {
            Position = new float3(0, 2, 0),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

}
}
