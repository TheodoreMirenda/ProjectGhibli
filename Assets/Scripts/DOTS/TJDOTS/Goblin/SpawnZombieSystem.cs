using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace TJ
{
    public struct ZombieSpawnTimer : IComponentData
    {
        public float Value;
    }
    [BurstCompile]
    public partial struct SpawnZombieSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ZombieSpawnTimer>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

            //schedule a job to run on the main thread on every frame
            new SpawnZombieJob
            {
                //passing the values into the job
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
            }.Run();//.Run() runs the job on the main thread, not really spawning, just decrementing the timer
        }
    }
    
    [BurstCompile]
    public partial struct SpawnZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer ECB;
        
        [BurstCompile]
        private void Execute(GraveyardAspect graveyard)
        {
            graveyard.ZombieSpawnTimer -= DeltaTime;
            if(!graveyard.TimeToSpawnZombie) return;
            if(!graveyard.ZombieSpawnPointInitialized()) return;
            
            graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
            var newZombie = ECB.Instantiate(graveyard.ZombiePrefab);

            var newZombieTransform = graveyard.GetZombieSpawnPoint();
            ECB.SetComponent(newZombie, newZombieTransform);
            
            var zombieHeading = MathHelpers.GetHeading(newZombieTransform.Position, graveyard.Position);
            ECB.SetComponent(newZombie, new ZombieHeading{Value = zombieHeading});
        }
    }
}