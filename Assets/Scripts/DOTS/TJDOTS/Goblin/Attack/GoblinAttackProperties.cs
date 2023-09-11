using Unity.Entities;
using Unity.Mathematics;

namespace TJ.DOTS
{
    //IEnableableComponent means we can enable/disable this component
    public struct GoblinAttackProperties : IComponentData, IEnableableComponent
    {
        public float AttackSpeed;
        public float AttackDamage;
        public float3 Position;
        public bool Cached;
    }
}