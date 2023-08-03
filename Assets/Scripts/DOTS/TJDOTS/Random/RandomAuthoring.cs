using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

public class RandomAuthoring : MonoBehaviour
{
    
}
public class RandomBaker : Baker<RandomAuthoring> {

    public override void Bake(RandomAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new RandomComponent {
            random = Unity.Mathematics.Random.CreateFromIndex(1)
        });
    }

}
