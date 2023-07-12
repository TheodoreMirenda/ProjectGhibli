using Unity.Burst;
using Unity.Entities;

namespace TMG.Zombies
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnZombieSystem))]//this is so that the zombie rise system runs after the spawn zombie system
    public partial struct ZombieRiseSystem : ISystem
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
            
            //schedule a job 
            new ZombieRiseJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()//.AsParallelWriter() allows the job to be run on multiple threads
            }.ScheduleParallel();//.ScheduleParallel() runs the job on multiple threads, the reason we are doing this is since
        }
    }

    [BurstCompile]
    public partial struct ZombieRiseJob : IJobEntity 
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]// [ChunkIndexInQuery] is used to get the index of the entity in the chunk so that we can remove the component or add a new one
        private void Execute(ZombieRiseAspect zombie, [ChunkIndexInQuery]int sortKey)
        {
            zombie.Rise(DeltaTime);//rise the zombie up
            if(!zombie.IsAboveGround) return; //if the zombie is not above ground, return
            
            zombie.SetAtGroundLevel();//set the zombie at ground level
            ECB.RemoveComponent<ZombieRiseRate>(sortKey, zombie.Entity);//want to remove the zombie rise rate component 
            ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);// add the zombie walk properties component
        }
    }

}