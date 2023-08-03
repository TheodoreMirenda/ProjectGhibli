using Unity.Entities;
using Unity.Mathematics;

namespace TJ.DOTS
{
    public struct GoblinTargetProperties : IComponentData, IEnableableComponent
    {
        public float3 TargetPosition;
    }
}