using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TJ.DOTS
{
    // [UpdateInGroup(typeof(InitializationSystemGroup))]//build in group
    public partial struct SpawnSystem : ISystem//ISystem is burst compatible
    {
        uint updateCounter;

        [BurstCompile]
        
        public void OnCreate(ref SystemState state)
        {
            // This call makes the system not update unless at least one entity in the world exists that has the Spawner component.
            // state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<CastleTag>();
            state.RequireForUpdate<Config>();
            // state.RequireForUpdate<GoblinSpawningProperties>();
            // state.RequireForUpdate<Execute.Prefabs>();
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state){
        }
        [BurstDiscard]
        public void Logg(int i = 0){
            UnityEngine.Debug.Log("SpawnSystem: " + i);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Create a query that matches all entities having a RotationSpeed component.
            // (The query is cached in source generation, so this does not incur a cost of recreating it every update.)
            var spinningCubesQuery = SystemAPI.QueryBuilder().WithAll<GoblinComponent>().Build();
            if (!spinningCubesQuery.IsEmpty) return;
            // Logg();

            //get CastleAspect 
            // var castle = SystemAPI.GetSingleton<CastleTag>();
            // var castlePosition = castle.CastleAspect.Position;
            

            // An EntityQueryMask provides an efficient test of whether a specific entity would
            // be selected by an EntityQuery.
            var queryMask = spinningCubesQuery.GetEntityQueryMask();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var config = SystemAPI.GetSingleton<Config>();
            var goblins = new NativeArray<Entity>(config.GoblinCount, Allocator.Temp);
            ecb.Instantiate(config.GoblinPrefab, goblins);

            foreach (var goblin in goblins)
            {
                var random = Unity.Mathematics.Random.CreateFromIndex(updateCounter++);
                float3 newPosition = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 50;
                newPosition.y = 0;

                var goblinPosition = new LocalTransform {
                        Position = newPosition,
                        Rotation = quaternion.identity,
                        Scale = 1
                };
                ecb.SetComponentForLinkedEntityGroup(goblin, queryMask, goblinPosition);
                //get random building
                // var randomBuilding = SystemAPI.GetSingleton<BuildingTag>();
                float3 goalPosition = (random.NextFloat3() - new float3(0.5f, 0, 0.5f)) * 50;
                goalPosition.y = 0;

                var goblinHeading = MathHelpers.GetHeading(goblinPosition.Position, goalPosition);
                ecb.SetComponent(goblin, new GoblinHeading{Value = goblinHeading, Offset = random.NextFloat(), Position = goalPosition});
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
