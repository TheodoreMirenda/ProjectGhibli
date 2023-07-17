using Unity.Entities;

namespace TJ.DOTS
{
    //IEnableableComponent means we can enable/disable this component
    public struct GoblinWalkProperties : IComponentData, IEnableableComponent
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
    }
    
    public struct GoblinEatProperties : IComponentData, IEnableableComponent
    {
        public float EatDamagePerSecond;
        public float EatAmplitude;
        public float EatFrequency;
    }
    
    public struct GoblinTimer : IComponentData
    {
        public float Value;
    }

    public struct GoblinHeading : IComponentData
    {
        public float Value;
    }
    
    public struct NewGoblinTag : IComponentData {}//allows us to find any new zombies that have been created
}