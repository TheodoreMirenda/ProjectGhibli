using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToFaceTest : MonoBehaviour
{
    public Transform target;

    [ContextMenu("Face Target")]
    public void FaceTarget()
    {
        //rotate only on the y axis
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(target.position - transform.position, Vector3.up).eulerAngles.y, 0);
        // transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
    }
}
