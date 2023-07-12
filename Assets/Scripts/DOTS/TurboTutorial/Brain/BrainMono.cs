using Unity.Entities;
using UnityEngine;

namespace TMG.Zombies
{
    public class BrainMono : MonoBehaviour
    {
        public float BrainHealth;
    }

    public class BrainBaker : Baker<BrainMono>
    {
        public override void Bake(BrainMono authoring)
        {
            var brainEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BrainTag>(brainEntity);
            AddComponent(brainEntity, new BrainHealth { Value = authoring.BrainHealth, Max = authoring.BrainHealth });            
            //since a bunch of zombies will be doing this across multiple threads, we cant just subtract from the health
            //so we add all the damage to a buffer, and then subtract from the health in the system
            AddBuffer<BrainDamageBufferElement>(brainEntity);
        }
    }
}