using Unity.Entities;

namespace TJ
{
    //IEnableableComponent means we can enable/disable this component
    public struct ZombieWalkProperties : IComponentData, IEnableableComponent
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
    }
    
    public struct ZombieEatProperties : IComponentData, IEnableableComponent
    {
        public float EatDamagePerSecond;
        public float EatAmplitude;
        public float EatFrequency;
    }
    
    public struct ZombieTimer : IComponentData
    {
        public float Value;
    }

    public struct ZombieHeading : IComponentData
    {
        public float Value;
    }
    
    public struct NewZombieTag : IComponentData {}//allows us to find any new zombies that have been created
}