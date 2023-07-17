using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
    [BurstCompile]
    public partial struct GoblinWalkSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            // var brainEntity = SystemAPI.GetSingletonEntity<Config>(); //empty entity that is used to get the brain position
            // var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
            // var brainRadius = brainScale * 5f + 0.5f;
            var brainRadius = 2.5f;
            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float BrainRadiusSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(GoblinWalkAspect goblin, [ChunkIndexInQuery] int sortKey)
        {
            goblin.Walk(DeltaTime);
            if (goblin.IsInStoppingRange(float3.zero, BrainRadiusSq))//if the zombie is in stopping range, chill with the walk and eat
            {
                // ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, false);
                ECB.DestroyEntity(sortKey, goblin.Entity);
                // ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            }
        }
    }

}