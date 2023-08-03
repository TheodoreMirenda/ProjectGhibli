using Unity.Entities;
using UnityEngine;

namespace TJ.DOTS
{
    public class BuildingMono : MonoBehaviour
    {
        public float BuildingHealth;
    }

    public class BuildingBaker : Baker<BuildingMono>
    {
        public override void Bake(BuildingMono authoring)
        {
            var buildingEntity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BuildingTag>(buildingEntity);
            //since a bunch of goblins will be damaging this across multiple threads, we cant just subtract from the health
            //so we add all the damage to a buffer, and then subtract from the health in the system
        }
    }

}