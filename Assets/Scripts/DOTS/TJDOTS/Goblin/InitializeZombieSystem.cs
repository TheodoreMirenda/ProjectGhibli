using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TJ
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitializeZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<NewZombieTag>())//gets all new zombies
            {
                ecb.RemoveComponent<NewZombieTag>(zombie.Entity);//removes the new zombie tag so we dont get them again
                ecb.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);//disables the zombie walk properties (until the zombie rises)
                ecb.SetComponentEnabled<ZombieEatProperties>(zombie.Entity, false);//disables the zombie eat properties (until the zombie is at the brain)
            }

            ecb.Playback(state.EntityManager);
        }
    }
}