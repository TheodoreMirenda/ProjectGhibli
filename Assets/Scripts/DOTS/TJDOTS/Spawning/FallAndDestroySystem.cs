using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
    public partial struct FallAndDestroySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<Execute.Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // An EntityCommandBuffer created from EntityCommandBufferSystem.Singleton will be
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

            // var fallAndDestroyJob = new FallAndDestroyJob
            // {
            //     ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
            //     DeltaTime = SystemAPI.Time.DeltaTime
            // };

            // fallAndDestroyJob.Schedule();
        }

        [BurstCompile]
        public partial struct FallAndDestroyJob : IJobEntity{
            public EntityCommandBuffer ECB;
            public float DeltaTime;
            public void Execute(Entity entity, in GoblinComponent goblinComponent, ref LocalTransform transform)
            {
                var movement = new float3(0, -DeltaTime * 5f, 0);

                //face the castle
                // var zombieHeading = MathHelpers.GetHeading(newZombieTransform.Position, SystemAPI.Query<CastleTag>.Position);
                // ECB.SetComponent(newZombie, new ZombieHeading{Value = zombieHeading});

                transform.Position += movement;
                if (transform.Position.y < 0) {
                    // Making a structural change would invalidate the query we are iterating through,
                    // so instead we record a command to destroy the entity later.
                    ECB.DestroyEntity(entity);
                }
            }
        }
    }
}
