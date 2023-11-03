using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TJ.DOTS
{
    public partial struct TowerSpawningSystem : ISystem
    {
        uint updateCounter;


        [BurstCompile]
        public void OnCreate(ref SystemState state) {
        }
        [BurstCompile]
        public void OnDestroy(ref SystemState state){
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            // (The query is cached in source generation, so this does not incur a cost of recreating it every update.)
            // var towerComponentQuery = SystemAPI.QueryBuilder().WithAll<SpawnNewTowerTag>().Build();
            // if (towerComponentQuery.IsEmpty) {
            //     UnityEngine.Debug.Log("No towers: " + towerComponentQuery.IsEmpty);
            //     return;
            // }
            // else {
            //     UnityEngine.Debug.Log("There are towers to spawn");
            // }

            // An EntityQueryMask provides an efficient test of whether a specific entity would
            // be selected by an EntityQuery.
            // var queryMask = towerComponentQuery.GetEntityQueryMask();
            // var ecb = new EntityCommandBuffer(Allocator.Temp);

            // var towerSpawningEntity = SystemAPI.GetSingletonEntity<TowerSpawningProperties>();
            // var towerSpawningAspect = SystemAPI.GetAspect<TowerSpawningAspect>(towerSpawningEntity);

            // var towers = new NativeArray<Entity>(1, Allocator.Temp);
            // ecb.Instantiate(towerSpawningAspect.TowerPrefab, towers);

            // foreach (var tower in towers)
            // {
            //     UnityEngine.Debug.Log($"towers to spawn: {towers.Length}");

            //     var towerPosition = towerSpawningAspect.GetRandomSpawningSpot();
            //     ecb.SetComponentForLinkedEntityGroup(tower, queryMask, towerPosition);
            //     // ecb.SetComponent(tower, new LocalTransform {
            //     //     Position = towerPosition.Position,
            //     //     Rotation = towerPosition.Rotation,
            //     //     Scale = towerPosition.Scale});

            //     // ecb.RemoveComponent<NewTowerTag>(tower);
            //     ecb.RemoveComponent<SpawnNewTowerTag>(tower);
                
            // }
            // ecb.Playback(state.EntityManager);
        }
    }
     public struct NewTowerTag : IComponentData {
        public LocalTransform localTransform;
    }
    public struct SpawnNewTowerTag : IComponentData {
        public LocalTransform localTransform;
    }
    
}
