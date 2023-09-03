using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace TJ.DOTS
{
    public readonly partial struct GoblinWalkAspect : IAspect
    {
        public readonly Entity Entity;
        
        private readonly RefRW<LocalTransform> _transform;
        public float3 CurrentPosition => _transform.ValueRO.Position;

        private readonly RefRW<GoblinTimer> _walkTimer;
        private readonly RefRO<GoblinWalkProperties> _walkProperties;
        private readonly RefRO<GoblinHeading> _heading;
        // private readonly RefRW<GoblinRandom> _goblinSpawningRandom;

        private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
        private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;
        private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
        private float3 Position => _heading.ValueRO.Position;
        private float Heading => _heading.ValueRO.Value;
        private float WalkTimerOffset => _heading.ValueRO.Offset;

        private float WalkTimer
        {
            get => _walkTimer.ValueRO.Value;
            set => _walkTimer.ValueRW.Value = value;
        }
        public void Walk(float deltaTime)
        {
            WalkTimer += deltaTime;
            _transform.ValueRW.Position += _transform.ValueRO.Forward() * WalkSpeed * deltaTime;
            
            //sway the zombie's body from side to side
            var swayAngle = WalkAmplitude * math.sin(WalkFrequency * (WalkTimer+WalkTimerOffset));
            _transform.ValueRW.Rotation = quaternion.Euler(0, Heading, 0);
            _transform.ValueRW.Position.y = swayAngle;
        }
        
        public bool IsInStoppingRange(float brainRadiusSq)
        {
            // return math.distancesq(brainPosition, _transform.ValueRW.Position) <= brainRadiusSq;
            return math.distancesq(Position, _transform.ValueRW.Position) <= brainRadiusSq;
        }
    }
}