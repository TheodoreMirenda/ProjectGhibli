using Unity.Entities;
using Unity.Mathematics;

namespace TJ.DOTS
{
    //IEnableableComponent means we can enable/disable this component
    public struct GoblinWalkProperties : IComponentData, IEnableableComponent
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
    }
    
    public struct GoblinTimer : IComponentData
    {
        public float Value;
    }

    public struct GoblinHeading : IComponentData
    {
        public float Value;
        public float Offset;
        public float3 Position;
    }
    
    public struct NewGoblinTag : IComponentData {}//allows us to find any new zombies that have been created 
}