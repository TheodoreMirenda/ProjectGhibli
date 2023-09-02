using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TJ.DOTS
{
public struct GoblinSpawningProperties : IComponentData
{
    public float2 FieldDimensions;
    public int NumberOfGoblinsToSpawn;
    public Entity GoblinPrefab;
    public float SafeZoneRadius;
}
public struct GoblinRandom : IComponentData
{
    public Unity.Mathematics.Random RandomValue;
}
}