using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{
    public class CastleMono : MonoBehaviour
    {
        public float CastleHealth;
    }
    public struct CastleHealth : IComponentData
    {
        public float Value;
        public float Max;
    }

    public class CastleBaker : Baker<CastleMono>
    {
        public override void Bake(CastleMono authoring)
        {
            var castleEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<CastleTag>(castleEntity);
            AddComponent(castleEntity, new CastleHealth { Value = authoring.CastleHealth, Max = authoring.CastleHealth });            
            //since a bunch of goblins will be damaging this across multiple threads, we cant just subtract from the health
            //so we add all the damage to a buffer, and then subtract from the health in the system
            AddBuffer<CastleDamageBufferElement>(castleEntity);
        }
    }

    [InternalBufferCapacity(8)]
    public struct CastleDamageBufferElement : IBufferElementData
    {
        public float Value;
    }
}