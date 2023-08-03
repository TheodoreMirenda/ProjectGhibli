using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
    public readonly partial struct GoblinTargetAspect : IAspect
    {
        public readonly Entity Entity;
        private readonly RefRO<LocalTransform> _transform;

        // private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
        // private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;
        // private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
        // private float3 Position => _heading.ValueRO.Position;
        // private float Heading => _heading.ValueRO.Value;
        // private float WalkTimerOffset => _heading.ValueRO.Offset;

        // private float WalkTimer
        // {
        //     get => _walkTimer.ValueRO.Value;
        //     set => _walkTimer.ValueRW.Value = value;
        // }
        public float3 Target()
        {
            
            return _transform.ValueRO.Position;
        }
        
        // public bool IsInStoppingRange(float3 brainPosition, float brainRadiusSq)
        // {
        //     // return math.distancesq(brainPosition, _transform.ValueRW.Position) <= brainRadiusSq;
        //     return math.distancesq(Position, _transform.ValueRW.Position) <= brainRadiusSq;
        // }
    }
}