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

    private readonly RefRO<GoblinSpawningProperties> _graveyardProperties;
    private readonly RefRW<GoblinRandom> _goblinSpawningRandom;

    public int NumberGoblinsToSpawn => _graveyardProperties.ValueRO.NumberOfGoblinsToSpawn;
    public Entity GoblinPrefab => _graveyardProperties.ValueRO.GoblinPrefab;

    public LocalTransform GetRandomTombstoneTransform() {
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
        } while (math.distancesq(Transform.Position, randomPosition) <=  _graveyardProperties.ValueRO.SafeZoneRadius);

        return randomPosition;
    }
    private float3 HalfDimensions => new()
    {
        x = _graveyardProperties.ValueRO.FieldDimensions.x * 0.5f,
        y = 0f,
        z = _graveyardProperties.ValueRO.FieldDimensions.y * 0.5f
    };
}
}
