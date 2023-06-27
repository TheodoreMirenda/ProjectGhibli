using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TJ
{
public class WindmillSpin : MonoBehaviour
{
    public GameObject spinningThing;
    public Vector3 rotationDirection;
    public float smoothTime;
    private float convertedTime = 200;
    private float smooth;
    void Update () {
        smooth = Time.deltaTime * smoothTime * convertedTime;
        spinningThing.transform.Rotate(rotationDirection * smooth, Space.Self);
        //spinningThing.transform.rotation = Quaternion.Euler(rotationDirection * smooth);
        
    }
}
}