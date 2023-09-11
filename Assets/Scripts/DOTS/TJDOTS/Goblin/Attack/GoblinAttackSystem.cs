using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TJ.DOTS
{
    [BurstCompile]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    public partial struct GoblinAttackSystem : ISystem
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
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var brainRadius = 2.5f;

            new GoblinAttackJob
            {
                DeltaTime = deltaTime,
                distanceToObject = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                // randomComponent = random
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinAttackJob : IJobEntity
    {
        public float DeltaTime;
        public float distanceToObject;
        public EntityCommandBuffer.ParallelWriter ECB;
        // public RefRW<RandomComponent> randomComponent;
        
        [BurstCompile]
        private void Execute(GoblinAttackAspect goblin, [ChunkIndexInQuery] int sortKey)
        {
            goblin.LockPosition();
            // goblin.Attack(DeltaTime);
        }
    }
}