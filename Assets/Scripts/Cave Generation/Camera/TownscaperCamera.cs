using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TJ
{
public class TownscaperCamera : MonoBehaviour
{
    private float X;
    private float Y;
    public float unlockedCameraZoom = -10f;
    public Camera mainCamera;
    public Transform cameraTarget;

    [Header("Camera Speeds")]
    [SerializeField] private float cameraRotationSpeed = 0.2f;
    [SerializeField] private float cameraYMovementSpeed = 10f, cameraPanSpeed = 1.5f;

    [Header("Max Camera Movement")]
    [SerializeField] private float minCameraZoom = -15f;
    [SerializeField] private float maxCameraZoom = -5f, maxCameraHeight = 10f, minCameraHeight = 0f, cameraLerpSpeed=1;
    
    InputHandler inputHandler;
    private void Awake()
    {
        inputHandler = FindObjectOfType<InputHandler>();
    }
    void Update()
    {
        inputHandler.TickInput(Time.deltaTime);

        if(inputHandler.UIOpen)
            return;
            
        HandleCameraRotation();
        HandleCameraZoom();
        HandleRaiseLowerCamera();
        HandlePanCamera(Time.deltaTime);
        HandleCameraMovement();

        // LockCameraMaximumRanges();

        this.transform.position = Vector3.Lerp(this.transform.position, cameraTarget.position, Time.deltaTime*cameraLerpSpeed);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, cameraTarget.rotation, Time.deltaTime*cameraLerpSpeed);
    }
    private void HandleCameraZoom()
    {
        float scroll_input = inputHandler.Scroll_input;

        if(scroll_input > 0 && unlockedCameraZoom < maxCameraZoom)
        {
            unlockedCameraZoom += (Time.deltaTime*100);
        }
        if(scroll_input < 0 && unlockedCameraZoom > minCameraZoom)
        {
            unlockedCameraZoom -= (Time.deltaTime*100);
        }

        if(inputHandler.Zoom_in_input && unlockedCameraZoom < maxCameraZoom)
        {
            unlockedCameraZoom += (Time.deltaTime*10);
        }
        if(inputHandler.Zoom_out_input && unlockedCameraZoom > minCameraZoom)
        {
            unlockedCameraZoom -= (Time.deltaTime*10);
        }
        
        Vector3 velocity = Vector3.zero;
        Vector3 cameraUnlockedPosition= new Vector3(0f,0f,unlockedCameraZoom);
        mainCamera.transform.localPosition = Vector3.SmoothDamp(mainCamera.transform.localPosition, cameraUnlockedPosition, ref velocity, Time.deltaTime*5);
    }
    private void HandleCameraRotation()
    {
        if(!inputHandler.MM_input) 
            return;

        cameraTarget.Rotate(new Vector3(-inputHandler.MouseY * cameraRotationSpeed, inputHandler.MouseX * cameraRotationSpeed, 0));
        X = cameraTarget.rotation.eulerAngles.x;
        Y = cameraTarget.rotation.eulerAngles.y;
        if(X>200)
            X=0;

        cameraTarget.rotation = Quaternion.Euler(Mathf.Clamp(X,0,90), Y, cameraTarget.rotation.z);
    }
    private void HandleRaiseLowerCamera()
    {
        if(inputHandler.Raise_Input)
        {
            if(transform.position.y < maxCameraHeight)
            {
                cameraTarget.position += new Vector3(0, cameraYMovementSpeed * Time.deltaTime, 0);
            }
        }
        if(inputHandler.Lower_Input)
        {
            if(transform.position.y > minCameraHeight)
            {
                cameraTarget.position -= new Vector3(0, cameraYMovementSpeed * Time.deltaTime, 0);
            }
        }
    }
    private void HandlePanCamera(float deltaTime)
    {
        if(!inputHandler.Pan_Input)
            return;
        
        //will pan either left or right or up or down
        Vector3 move = new Vector3(-inputHandler.MouseX, -inputHandler.MouseY, 0);
        cameraTarget.Translate(move * deltaTime * cameraPanSpeed, Space.Self);
    }
    private void HandleCameraMovement()
    {
        //get the current rotation of the camera
        Quaternion currentRotation = cameraTarget.rotation;
        //set the y rotation to 0 so we only have the x and z rotation
        currentRotation.eulerAngles = new Vector3(0, currentRotation.eulerAngles.y, 0);

        //get the current input direction
        Vector3 inputDirection = new Vector3(inputHandler.MovementInput.x, 0, inputHandler.MovementInput.y);
        //rotate the input direction by the camera rotation
        Vector3 rotatedInputDirection = currentRotation * inputDirection;
        //move the camera by the rotated input direction
        cameraTarget.Translate(rotatedInputDirection * Time.deltaTime * cameraPanSpeed, Space.World);
    }
}
}