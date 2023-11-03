using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InitializeTowerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // var ecb = new EntityCommandBuffer(Allocator.Temp);
            // foreach (var tower in SystemAPI.Query<TowerSpawningAspect>().WithAll<NewTowerTag>())
            // {
            //     UnityEngine.Debug.Log("InitializeTowerSystem: " + tower.Entity);
            //     ecb.RemoveComponent<NewTowerTag>(tower.Entity);
            // }

            // ecb.Playback(state.EntityManager);
        }
    }
}