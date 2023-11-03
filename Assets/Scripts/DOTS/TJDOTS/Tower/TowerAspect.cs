using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace TJ.DOTS
{
    public readonly partial struct TowerAspect : IAspect
    {
        public readonly Entity Entity;
        private readonly RefRO<LocalTransform> _transformAspect;
        private LocalTransform Transform => _transformAspect.ValueRO;
        public float3 Position => Transform.Position;
    }
}
