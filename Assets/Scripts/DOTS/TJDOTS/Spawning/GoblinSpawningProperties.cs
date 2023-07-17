using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
public struct GoblinSpawningProperties : IComponentData
{
    public float2 FieldDimensions;
    public int NumberOfGoblinsToSpawn;
    public Entity GoblinPrefab;
}
public struct GoblinSpawningRandom : IComponentData
{
    public Random Value;
}
}