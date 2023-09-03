using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TJ.DOTS
{
public struct GoblinSpawningProperties : IComponentData
{
    public Entity GoblinPrefab;
    public float2 FieldDimensions;
    public int NumberOfGoblinsToSpawn;
    public float SafeZoneRadius;
}
public struct GoblinRandom : IComponentData
{
    public Unity.Mathematics.Random RandomValue;
}
}