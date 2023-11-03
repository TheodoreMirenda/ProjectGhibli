using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{

    public class TowerMono : MonoBehaviour
    {
        
    }

    public class TowerBaker : Baker<TowerMono> 
    {
        public override void Bake(TowerMono authoring)
        {
            // RefRW<RandomComponent> random =  SystemAPI.GetSingletonRW<RandomComponent>();
            var towerEntity = GetEntity(TransformUsageFlags.Dynamic);

            // AddComponent(goblinEntity, new GoblinComponent{});
            // AddComponent(goblinEntity, new GoblinWalkProperties
            // {
            //     WalkSpeed = authoring.WalkSpeed,
            //     WalkAmplitude = authoring.WalkAmplitude,
            //     WalkFrequency = authoring.WalkFrequency
            // });

            // AddComponent<GoblinTimer>(goblinEntity);
            // AddComponent<GoblinHeading>(goblinEntity);
            AddComponent<NewTowerTag>(towerEntity);
            UnityEngine.Debug.Log("TowerBaker, towerEntity: " + towerEntity);
            

            // AddComponent(goblinEntity, new GoblinAttackProperties{
            //     AttackSpeed = authoring.AttackSpeed,
            //     AttackDamage = authoring.AttackDamage
            // });
            // AddComponent(goblinEntity, new GoblinRandom{RandomValue = random.ValueRW.random});
        }
        // public float GetOffset(RefRW<RandomComponent> randomComponent)
        // {
        //     return randomComponent.ValueRW.random.NextFloat();
        // }
    }
}