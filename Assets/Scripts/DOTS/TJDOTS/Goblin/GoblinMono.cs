using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{
    public class GoblinMono : MonoBehaviour
    {
        public float RiseRate;
        public float WalkSpeed;
        public float WalkAmplitude;
        public float WalkFrequency;
        
        public float EatDamage;
        public float EatAmplitude;
        public float EatFrequency;
    }

    public class GoblinBaker : Baker<GoblinMono> 
    {
        public override void Bake(GoblinMono authoring)
        {
            var goblinEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(goblinEntity, new GoblinComponent{});
            AddComponent(goblinEntity, new GoblinWalkProperties
            {
                WalkSpeed = authoring.WalkSpeed,
                WalkAmplitude = authoring.WalkAmplitude,
                WalkFrequency = authoring.WalkFrequency
            });
            // AddComponent(goblinEntity, new ZombieEatProperties
            // {
            //     EatDamagePerSecond = authoring.EatDamage,
            //     EatAmplitude = authoring.EatAmplitude,
            //     EatFrequency = authoring.EatFrequency
            // });
            AddComponent<GoblinTimer>(goblinEntity);
            AddComponent<GoblinHeading>(goblinEntity);
            AddComponent<NewGoblinTag>(goblinEntity);
        }
    }
    public struct GoblinComponent : IComponentData {
    }
}