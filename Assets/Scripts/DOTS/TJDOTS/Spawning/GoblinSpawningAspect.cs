using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
public readonly partial struct GoblinSpawningAspect : IAspect
{
    public readonly Entity Entity;
    private readonly RefRO<LocalTransform> _transformAspect;
    private LocalTransform Transform => _transformAspect.ValueRO;

    private readonly RefRO<GoblinSpawningProperties> _goblinSpawningProperties;
    private readonly RefRW<GoblinRandom> _goblinSpawningRandom;

    public int NumberGoblinsToSpawn => _goblinSpawningProperties.ValueRO.NumberOfGoblinsToSpawn;
    public Entity GoblinPrefab => _goblinSpawningProperties.ValueRO.GoblinPrefab;

    public LocalTransform GetRandomSpawningSpot() {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }
    private float3 MinCorner => Transform.Position - HalfDimensions;
    private float3 MaxCorner => Transform.Position + HalfDimensions;
    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _goblinSpawningRandom.ValueRW.RandomValue.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(Transform.Position, randomPosition) <=  _goblinSpawningProperties.ValueRO.SafeZoneRadius);

        return randomPosition;
    }
    private float3 HalfDimensions => new()
    {
        x = _goblinSpawningProperties.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = _goblinSpawningProperties.ValueRO.FieldDimensions.y * 0.5f
    };
}
}
