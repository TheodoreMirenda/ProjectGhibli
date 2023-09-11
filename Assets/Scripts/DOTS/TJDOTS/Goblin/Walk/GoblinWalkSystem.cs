using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TJ.DOTS
{
    [BurstCompile]
    // [UpdateAfter(typeof(InitializeGoblinSystem))]
    [UpdateAfter(typeof(SpawnSystem))]//this is so that the zombie rise system runs after the spawn zombie system
    public partial struct GoblinWalkSystem : ISystem
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
            // RefRW<RandomComponent> random =  SystemAPI.GetSingletonRW<RandomComponent>();
            var brainRadius = 10f;
            // Debug.Log($"GoblinWalkJob: ");

            new GoblinWalkJob
            {
                DeltaTime = deltaTime,
                distanceToObject = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                // randomComponent = random
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float distanceToObject;
        public EntityCommandBuffer.ParallelWriter ECB;
        // public RefRW<RandomComponent> randomComponent;
        
        [BurstCompile]
        private void Execute(GoblinWalkAspect goblin, [ChunkIndexInQuery] int sortKey)
        {
            goblin.Walk(DeltaTime);
            if (goblin.IsInStoppingRange(distanceToObject))//if the zombie is in stopping range, chill with the walk and eat
            {
                // ECB.DestroyEntity(sortKey, goblin.Entity);
                goblin.CachePosition();
                // ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, false);
                // ECB.SetComponentEnabled<GoblinAttackProperties>(sortKey, goblin.Entity, true);
            }
        }
    }
}