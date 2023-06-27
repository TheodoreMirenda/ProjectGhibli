using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public struct Weights
{
    [Range(0f,99f)] public float roadThreshold;
    public List<PrototypeWeight> prototypeWeights;
    public int GetWeight(Prototype prototype)
    {
        foreach (var item in prototypeWeights)
        {
            if (item.prototype == prototype)
            {
                return (int)item.weight;
            }
        }
        //standard weight
        return 5;
    }
}
[System.Serializable] public struct PrototypeWeight
{
    public Prototype prototype;
    [Range(0f,10f)] public float weight;
}
public enum WFC_Attribute {Intersection, RoadStraight, Water, WaterBend};
