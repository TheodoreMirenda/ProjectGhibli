using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace TMG.Zombies
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]//build in group
    public partial struct SpawnTombstoneSystem : ISystem //ISystem is burst compatible
    {
        //has a lifecycle OnCreate -> OnUpdate -> OnDestroy
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //update function if only one entity exists in application
            state.RequireForUpdate<GraveyardProperties>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false; //dont update every frame, basically used like an awake function
            
            var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
            var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            //Allocator.Temp will be freed at the end of the frame
            //Allocator.TempJob will be freed at the end of 4 frames
            //Allocator.Persistent will not be freed until the game is closed

            var builder = new BlobBuilder(Allocator.Temp);
            ref var spawnPoints = ref builder.ConstructRoot<ZombieSpawnPointsBlob>();
            var arrayBuilder = builder.Allocate(ref spawnPoints.Value, graveyard.NumberTombstonesToSpawn);

            var tombstoneOffset = new float3(0f, -2f, 1f);
            
            for (var i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
            {
                var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);
                var newTombstoneTransform = graveyard.GetRandomTombstoneTransform();
                ecb.SetComponent(newTombstone, newTombstoneTransform);
                
                var newZombieSpawnPoint = newTombstoneTransform.Position + tombstoneOffset;
                arrayBuilder[i] = newZombieSpawnPoint;
            }

            //Allocator.Persistent will not be freed until the game is closed
            var blobAsset = builder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
            ecb.SetComponent(graveyardEntity, new ZombieSpawnPoints{Value = blobAsset});
            builder.Dispose();

            // graveyard.ZombieSpawnPoints = spawnPoints.ToArray(Allocator.Persistent);
            
            ecb.Playback(state.EntityManager);
        }
    }
}