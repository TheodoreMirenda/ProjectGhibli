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
            // state.RequireForUpdate<Config>();
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
            var brainRadius = 2.5f;

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
                float3 goalPosition = goblin.GetRandomPosition();
                goalPosition.y = 0;
                var goblinHeading = MathHelpers.GetHeading(goblin.CurrentPosition, goalPosition);

                ECB.SetComponent(sortKey, goblin.Entity, new GoblinHeading{Value = goblinHeading, Offset = goblin.GetOffset(), Position = goalPosition});
            }
        }
                
        //UnityEngine.Random.Range(0f, 15f) doesnt work in burst, need to use mathematics library
        
        
    }

                // random = SystemAPI.GetSingleton<RandomComponent>().random 
                //actual single not being modified, are you working with a copy or the data


}