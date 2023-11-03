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
    [UpdateBefore(typeof(TransformSystemGroup))]// if you want enties to be spawned before the transform system group runs
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

            // var minDist = config.ObstacleRadius + 0.5f; // the player capsule radius is 0.5f
            // var minDistSQ = minDist * minDist;

            // For every entity having a LocalTransform and Player component, a read-write reference to
            // the LocalTransform is assigned to 'playerTransform'.
            foreach (var playerTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<GoblinWalkAspect>()) {
                // var newPos = playerTransform.ValueRO.Position + input;

                // // A foreach query nested inside another foreach query.
                // // For every entity having a LocalTransform and Obstacle component, a read-only reference to
                // // the LocalTransform is assigned to 'obstacleTransform'.
                // foreach (var obstacleTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacle>())
                // {
                //     // If the new position intersects the player with a wall, don't move the player.
                //     if (math.distancesq(newPos, obstacleTransform.ValueRO.Position) <= minDistSQ)
                //     {
                //         newPos = playerTransform.ValueRO.Position;
                //         // break;
                //     }
                // }

                // playerTransform.ValueRW.Position = newPos;
            }

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
            //collision check


            if (!goblin.IsInStoppingRange(distanceToObject)) {
                goblin.Walk(DeltaTime);
            }
            else{
                // ECB.DestroyEntity(sortKey, goblin.Entity);
                // goblin.CachePosition();
                ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, false);
                ECB.SetComponentEnabled<GoblinAttackProperties>(sortKey, goblin.Entity, true);
            }
        }
    }
}