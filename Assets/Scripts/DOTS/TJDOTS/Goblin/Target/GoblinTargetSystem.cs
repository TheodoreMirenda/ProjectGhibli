using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TJ.DOTS
{
    [BurstCompile]
    [UpdateAfter(typeof(GoblinWalkSystem))]
    public partial struct GoblinTargetSystem : ISystem
    {
        private readonly RefRO<LocalTransform> _transformAspect;
        // private LocalTransform Transform => _transformAspect.ValueRO;
        // public float3 Position => Transform.Position;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

            // var deltaTime = SystemAPI.Time.DeltaTime;
            // var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            // //quewry for all buildings
            // // var brainEntity = SystemAPI.GetSingletonEntity<Config>(); //empty entity that is used to get the brain position
            // // var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;

            // new GoblinTargetJob
            // {
            //     currentPosition = Position,
            //     goalPosition = new float3(0,0,0),
            //     ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            // }.ScheduleParallel();

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // Debug.Log($"GoblinTargetSystem state: {state.Enabled}");
            // state.Enabled = false; //dont update every frame, basically used like an awake function


            // var deltaTime = SystemAPI.Time.DeltaTime; //cant use systemapi inside of aspect
            // var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            // new GoblinTargetJob
            // {
            //     goalPosition = new float3(-10,0,10),
            //     ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            // }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct GoblinTargetJob : IJobEntity
    {
        public float3 goalPosition;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(GoblinTargetAspect goblin, [ChunkIndexInQuery] int sortKey)
        {
            //add turning logic here
            // goblin.Target();
            // if (goblin.IsInStoppingRange(float3.zero, BrainRadiusSq))
            // {
                
                var random = Unity.Mathematics.Random.CreateFromIndex((uint)sortKey+1);
                float3 goalPosition = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 5;
                goalPosition.y = 0;

                var goblinHeading = MathHelpers.GetHeading(goblin.Target(), goalPosition);

                ECB.SetComponent(sortKey, goblin.Entity, new GoblinHeading{Value = goblinHeading, Offset = random.NextFloat(), Position = goalPosition});
                ECB.SetComponentEnabled<GoblinTargetProperties>(sortKey, goblin.Entity, false);
                ECB.SetComponentEnabled<GoblinWalkProperties>(sortKey, goblin.Entity, true);
            // }
        }
    }

}