using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public MeshFilter tileMeshFilter;
    public GameObject decorations;
    public Transform trueNorth;
    public Decoration[] decorationData;

    [ContextMenu("Set Up")]
    public void FaceTarget()
    {
        if(trueNorth==null)
        {
            Debug.Log("trueNorth is null");
            return;
        }

        foreach(Decoration d in decorationData)
        {
            d.decorationGameObject.rotation = Quaternion.Euler(0, Quaternion.LookRotation(trueNorth.position 
                - d.decorationGameObject.transform.position, Vector3.up).eulerAngles.y, 0);
        }
    }
}

[System.Serializable] public struct Decoration{
    public Transform decorationGameObject;
    public float activationChance;
}
