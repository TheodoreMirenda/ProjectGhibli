using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace TJ.DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitializeGoblinSystem : ISystem
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
            foreach (var goblin in SystemAPI.Query<GoblinWalkAspect>().WithAll<NewGoblinTag>())
            {
                ecb.RemoveComponent<NewGoblinTag>(goblin.Entity);
                ecb.SetComponentEnabled<GoblinTargetProperties>(goblin.Entity, false);//disables the zombie eat properties (until the zombie is at the brain)
                
            }

            ecb.Playback(state.EntityManager);
        }
    }
}