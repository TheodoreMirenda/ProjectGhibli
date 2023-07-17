using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
public readonly partial struct GoblinSpawningAspect : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<GoblinSpawningProperties> _graveyardProperties;
    private readonly RefRW<GoblinSpawningRandom> _graveyardRandom;
    private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
    private readonly RefRO<LocalTransform> _transformAspect;


    private const float BRAIN_SAFETY_RADIUS_SQ = 100;//area around brain that zombies cant spawn in
    private LocalTransform Transform => _transformAspect.ValueRO;


    private float3 MinCorner => Transform.Position - HalfDimensions;
    private float3 MaxCorner => Transform.Position + HalfDimensions;
    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(Transform.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

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
