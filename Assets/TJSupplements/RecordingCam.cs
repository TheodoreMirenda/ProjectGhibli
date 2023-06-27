using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingCam : MonoBehaviour
{
    
    private float X;
    private float Y;
    public float unlockedCameraZoom = -10f;
    public float minCameraZoom = -15f;
    public float maxCameraZoom = -5f;
    public Camera cameraTransform;
    public float smooth;

    [Header("Move Camera")]
    public bool moveCamera;
    public float moveSpeed = 3.5f;
    public Direction direction;
    public enum Direction {Left,Right,Up,Down};
    
    [Header("Rotate Around")]
    public bool rotateAround;
    public Transform targetTransform;
    private Transform myTransform;
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public float rotateAroundSpeed = 0.1f;
    public float followSpeed = 0.1f;


    void Update()
    {
        // if(Input.GetMouseButton(0)) 
        //     HandleCameraRotation();

        // HandleCameraZoom();

        if(moveCamera)
            SimpleMoveToRecord();

        if(rotateAround)
            HandleCameraRotation();
        // HandleCameraRotation(float delta, float mouseXInput, float mouseYInput);
    }
    public void followTarget(float delta){
       Vector3 targetPosition = Vector3.SmoothDamp
            (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
       myTransform.position = targetPosition;

       //HandleCameraCollisions(delta);
   }
    private void SimpleMoveToRecord()
    {
        Vector3 d = new Vector3();
        if(direction==Direction.Left)
            d = Vector3.forward;
        else if(direction==Direction.Right)
            d = Vector3.back;
        if(direction==Direction.Up)
            d = Vector3.up;
        else if(direction==Direction.Down)
            d = Vector3.down;

        targetTransform.transform.Translate(d * Time.deltaTime / moveSpeed, Space.World);
    }
    private void HandleCameraZoom()
    {
        float scroll_input = Input.mouseScrollDelta.y;
        if(scroll_input>0&&unlockedCameraZoom<maxCameraZoom)
            unlockedCameraZoom+=(Time.deltaTime*100);
        if(scroll_input<0&&unlockedCameraZoom>minCameraZoom)
            unlockedCameraZoom-=(Time.deltaTime*100);
        
        Vector3 velocity = Vector3.zero;
        Vector3 cameraUnlockedPosition= new Vector3(0f,0f,unlockedCameraZoom);
        cameraTransform.transform.localPosition = Vector3.SmoothDamp(cameraTransform.transform.localPosition, cameraUnlockedPosition, ref velocity, Time.deltaTime*5);
    }
    private void HandleCameraRotation()
    {
        // transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y") * speed, Input.GetAxis("Mouse X") * speed, 0));
        // X = transform.rotation.eulerAngles.x;
        // Y = transform.rotation.eulerAngles.y;
        // if(X>200)
        //     X=0;

        // transform.rotation = Quaternion.Euler(Mathf.Clamp(X,0,60), Y, transform.rotation.z);

        transform.RotateAround(targetTransform.transform.position, Vector3.up, rotateAroundSpeed * Time.deltaTime);
    }
}
