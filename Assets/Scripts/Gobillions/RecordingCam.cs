using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordingCam : MonoBehaviour
{
    [Tooltip("Should be a child of this object")]
    public Camera cameraTransform;
    [SerializeField] private Vector3 cameraPositionOverride = new (0,10,-10);
    [SerializeField] private Vector3 cameraRotationOverride = new (30,0,0);

    // [Header("Zoom Camera")]
    // public float unlockedCameraZoom = -10f;
    // public float minCameraZoom = -15f;
    // public float maxCameraZoom = -5f;
    // public float smooth;

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

    private Vector3 cachedCameraPosition, cachedCameraRotation;
    
    private void Start() {
        myTransform = transform;
        cachedCameraPosition = cameraTransform.transform.localPosition;
        cachedCameraRotation = cameraTransform.transform.localRotation.eulerAngles;
    }

    void Update()
    {
        if(cachedCameraPosition!=cameraPositionOverride||cachedCameraRotation!=cameraRotationOverride){
            cachedCameraPosition = cameraPositionOverride;
            cachedCameraRotation = cameraRotationOverride;
            cameraTransform.transform.SetLocalPositionAndRotation(cameraPositionOverride, Quaternion.Euler(cameraRotationOverride));
        }

        // if(Input.GetMouseButton(0)) 
        //     HandleCameraRotation();

        // HandleCameraZoom();

        if(moveCamera)
            SimpleMoveToRecord();

        if(rotateAround)
            HandleCameraRotation();

        
    }
    public void FollowTarget(float delta){
       Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
       myTransform.position = targetPosition;

       //HandleCameraCollisions(delta);
   }
    private void SimpleMoveToRecord() {
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
    // private void HandleCameraZoom() {
    //     float scroll_input = Input.mouseScrollDelta.y;
    //     if(scroll_input>0&&unlockedCameraZoom<maxCameraZoom)
    //         unlockedCameraZoom+=Time.deltaTime*100;
    //     if(scroll_input<0&&unlockedCameraZoom>minCameraZoom)
    //         unlockedCameraZoom-=Time.deltaTime*100;
        
    //     Vector3 velocity = Vector3.zero;
    //     Vector3 cameraUnlockedPosition= new (0f,0f,unlockedCameraZoom);
    //     cameraTransform.transform.localPosition = Vector3.SmoothDamp(cameraTransform.transform.localPosition, cameraUnlockedPosition, ref velocity, Time.deltaTime*5);
    // }
    private void HandleCameraRotation() {
        transform.RotateAround(targetTransform.transform.position, Vector3.up, rotateAroundSpeed * Time.deltaTime);
    }
}
