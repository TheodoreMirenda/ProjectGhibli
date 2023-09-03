using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{

    public class GoblinMono : MonoBehaviour
    {
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
        public uint RandomSeed;
        
        // public float EatDamage;
        // public float EatAmplitude;
        // public float EatFrequency;
    }

    public class GoblinBaker : Baker<GoblinMono> 
    {
        public override void Bake(GoblinMono authoring)
        {
            // RefRW<RandomComponent> random =  SystemAPI.GetSingletonRW<RandomComponent>();
            var goblinEntity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(goblinEntity, new GoblinComponent{});
            AddComponent(goblinEntity, new GoblinWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });
            // AddComponent(goblinEntity, new GoblinTargetProperties{});

            AddComponent<GoblinTimer>(goblinEntity);
            AddComponent<GoblinHeading>(goblinEntity);
            AddComponent<NewGoblinTag>(goblinEntity);
            // AddComponent(goblinEntity, new GoblinRandom{RandomValue = random.ValueRW.random});
        }
        // public float GetOffset(RefRW<RandomComponent> randomComponent)
        // {
        //     return randomComponent.ValueRW.random.NextFloat();
        // }
    }
    public struct GoblinComponent : IComponentData {
    }
    
}