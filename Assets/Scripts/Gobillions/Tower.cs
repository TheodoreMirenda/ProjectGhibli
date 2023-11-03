using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace TJ.DOTS
{

public class Tower : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TowerPrefab;
    void Start()
    {
        Entity entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();
        // World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<TowerEntity>(entity);
        // World.DefaultGameObjectInjectionWorld.EntityManager.GetBuffer<TowerEntity>(entity, new TowerPlacementInput { Value = transform.position });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}