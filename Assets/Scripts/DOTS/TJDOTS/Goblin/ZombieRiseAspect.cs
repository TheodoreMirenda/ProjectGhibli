using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ
{
    public readonly partial struct ZombieRiseAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<LocalTransform> _transformAspect;
        private readonly RefRO<ZombieRiseRate> _zombieRiseRate;

        public void Rise(float deltaTime)
        {
            //modify the position of the zombie to make it rise
            _transformAspect.ValueRW.Position += math.up() * _zombieRiseRate.ValueRO.Value * deltaTime;
        }
        
        //once the zombie is above ground, it can start moving towards the brain
        public bool IsAboveGround => _transformAspect.ValueRW.Position.y >= 0f;

        public void SetAtGroundLevel()
        {
            var position = _transformAspect.ValueRW.Position;
            position.y = 0f;
            _transformAspect.ValueRW.Position = position;
        }
    }
}