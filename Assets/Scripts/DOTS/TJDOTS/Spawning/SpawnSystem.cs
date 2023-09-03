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
            // This call makes the system not update unless at least one entity in the world exists that has the GoblinSpawningProperties component.
            state.RequireForUpdate<CastleTag>();
            state.RequireForUpdate<GoblinSpawningProperties>();
            state.RequireForUpdate<CastleTag>();
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
            var goblinComponentQuery = SystemAPI.QueryBuilder().WithAll<GoblinComponent>().Build();
            if (!goblinComponentQuery.IsEmpty) return;
            // Logg();

            //get CastleAspect 
            var castle = SystemAPI.GetSingletonEntity<CastleTag>();
            var castlePosition = SystemAPI.GetComponent<LocalTransform>(castle).Position;
            
            // An EntityQueryMask provides an efficient test of whether a specific entity would
            // be selected by an EntityQuery.
            var queryMask = goblinComponentQuery.GetEntityQueryMask();
            var ecb = new EntityCommandBuffer(Allocator.Temp);


            var goblinSpawningEntity = SystemAPI.GetSingletonEntity<GoblinSpawningProperties>();
            var goblinSpawningAspect = SystemAPI.GetAspect<GoblinSpawningAspect>(goblinSpawningEntity);

            var goblins = new NativeArray<Entity>(goblinSpawningAspect.NumberGoblinsToSpawn, Allocator.Temp);
            ecb.Instantiate(goblinSpawningAspect.GoblinPrefab, goblins);

            foreach (var goblin in goblins)
            {
                var random = Unity.Mathematics.Random.CreateFromIndex(updateCounter++);

                var goblinPosition = goblinSpawningAspect.GetRandomSpawningSpot();
                ecb.SetComponentForLinkedEntityGroup(goblin, queryMask, goblinPosition);
                //get random building
                // var randomBuilding = SystemAPI.GetSingleton<BuildingTag>();

                var goblinHeading = MathHelpers.GetHeading(goblinPosition.Position, castlePosition);
                ecb.SetComponent(goblin, new GoblinHeading{ Value = goblinHeading, 
                                                            Offset = random.NextFloat(), 
                                                            Position = castlePosition
                                                            });
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
