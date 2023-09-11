using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
    public readonly partial struct GoblinAttackAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        private readonly RefRO<GoblinAttackProperties> _attackProperties;
        public void LockPosition()
        {
            if(_attackProperties.ValueRO.Cached)
                _transform.ValueRW.Position = _attackProperties.ValueRO.Position;
        }
    }
}
