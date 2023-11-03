// using Unity.Entities;
// using UnityEngine;

// namespace TJ.DOTS
// {
//     public class TowerMono : MonoBehaviour
//     {
//         public float CastleHealth;
//     }
//     public struct TowerHealth : IComponentData
//     {
//         public float Value;
//         public float Max;
//     }

//     public class TowerBaker : Baker<CastleMono>
//     {
//         public override void Bake(CastleMono authoring)
//         {
//             var castleEntity = GetEntity(TransformUsageFlags.Dynamic);
//             AddComponent<CastleTag>(castleEntity);
//             AddComponent<BuildingTag>(castleEntity);
//             // AddComponent(castleEntity, new TowerHealth { Value = authoring.CastleHealth, Max = authoring.CastleHealth });            
//             //since a bunch of goblins will be damaging this across multiple threads, we cant just subtract from the health
//             //so we add all the damage to a buffer, and then subtract from the health in the system
//             AddBuffer<CastleDamageBufferElement>(castleEntity);
//         }
//     }
// }