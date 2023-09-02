using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TJ
{
public class InputHandler : MonoBehaviour
{
    [SerializeField] private ClickableTile cubeSelected, cubeBeingDisplayed;
    [SerializeField] private LayerMask clickableLayer, defaultLayer, notClickableLayer;
    public CursorMode cursorMode;
    public enum CursorMode {Select, Build, NotYourTurn}
    [SerializeField] private Vector3 mousePosition;
    public Vector3 MousePosition => mousePosition;
    private float horizontal;
    // public float Horizontal => horizontal;
    private float vertical;
    // public float Vertical => vertical;
    private float moveAmount;
    [SerializeField] private float mouseX;
    public float MouseX => mouseX;
    [SerializeField] private float mouseY;
    public float MouseY => mouseY;
    [SerializeField] private float scroll_input;
    public float Scroll_input => scroll_input;
    [SerializeField] private bool zoom_in_input;
    public bool Zoom_in_input => zoom_in_input;
    [SerializeField] private bool zoom_out_input;
    public bool Zoom_out_input => zoom_out_input;
    [SerializeField] private bool raise_Input;
    public bool Raise_Input => raise_Input;
    [SerializeField] private bool lower_Input;
    public bool Lower_Input => lower_Input;

    [SerializeField] private bool camera_Rotation_Input;
    public bool MM_input => camera_Rotation_Input;

    [SerializeField] private bool pan_Input;
    public bool Pan_Input => pan_Input;

    private bool r_input;
    private bool leftClick;
    public bool LeftClick => leftClick;
    private bool rightClick;
    public bool RightClick => rightClick;
    private bool esc_input;

    [SerializeField] private Vector2 movementInput;
    public Vector2 MovementInput => movementInput;
    [SerializeField] private Vector2 cameraInput;
    // public Vector2 CameraInput => cameraInput;
    RaycastHit hit;
    Ray ray;
    GameControls inputActions;
    // public GameControls InputActions => inputActions;
    public bool UIOpen;
    private Coroutine zoomCoroutine;
    public bool zooming;
    [SerializeField] private GameCursor gameCursor;
    [SerializeField] private GameBoard gameboard;
    // Vector2 touch2;

    private void Awake()
    {
        // buildingSystem = FindObjectOfType<BuildingSystem>();
        // uIManager = FindObjectOfType<UIManager>();
        // orthoCameraController = FindObjectOfType<OrthoCameraController>();
    }
    public void OnEnable(){
        if (inputActions == null){
            inputActions = new GameControls();
            inputActions.GameMain.Primary.performed += i => leftClick = true;
            inputActions.GameMain.Secondary.performed += i => rightClick = true;

            // inputActions.GameMain.TouchPosition.performed += x => CallSomething(x.ReadValue<Vector2>());

            inputActions.GameMain.CameraRotation.performed+= i => camera_Rotation_Input = true;
            inputActions.GameMain.CameraRotation.canceled += i => camera_Rotation_Input = false;
            inputActions.GameMain.Camera.performed+= i => cameraInput = i.ReadValue<Vector2>();

            inputActions.GameMain.RaiseCamera.performed+= i => raise_Input = true;
            inputActions.GameMain.RaiseCamera.canceled += i => raise_Input = false;
            inputActions.GameMain.LowerCamera.performed+= i => lower_Input = true;
            inputActions.GameMain.LowerCamera.canceled += i => lower_Input = false;
            inputActions.GameMain.Movement.performed+= inputActions => movementInput = inputActions.ReadValue<Vector2>();

            inputActions.GameMain.PanCamera.performed += i => pan_Input = true;
            inputActions.GameMain.PanCamera.canceled += i => pan_Input = false;

            inputActions.GameMain.Mouse.performed+= inputActions => mousePosition = inputActions.ReadValue<Vector2>();
            
            // inputActions.GameMain.RotateBuilding.performed+= i => r_input = true;
            // inputActions.GameMain.Escape.performed+= i => esc_input = true;
            
            // inputActions.GameMain.CameraZoom.performed+= i => scroll_input = i.ReadValue<float>();
            // inputActions.GameMain.ZoomIn.performed+= i => zoom_in_input = true;
            // inputActions.GameMain.ZoomIn.canceled += i => zoom_in_input = false;
            // inputActions.GameMain.ZoomOut.performed+= i => zoom_out_input = true;
            // inputActions.GameMain.ZoomOut.canceled += i => zoom_out_input = false;

            // inputActions.GameMain.TouchPosition.started += x => StartDrag(x.ReadValue<Vector2>());
            // inputActions.GameMain.Drag.canceled += x => EndDrag();
            // inputActions.GameMain.Drag.started += x => StartDrag(x.ReadValue<Vector2>());
            // inputActions.GameMain.Drag.performed += x => orthoCameraController.Drag(x.ReadValue<Vector2>());
            // inputActions.GameMain.Drag.canceled += x => EndDrag();
        }
        inputActions.Enable();
    }

    private void OnDisable(){
        inputActions.Disable();
    }
    public void TickInput(float delta){
        // Debug.Log($"tick{delta}");
        HandleMoveInput(delta);
        HandleRotatBuildingInput();
        // DisplayCursor();
        HandleMouseInput();
        HandleEscape();
    }
    private void HandleMoveInput(float delta){
        if(UIOpen)
            return;

        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal)+Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    private void LateUpdate()
    {
        leftClick = false;
        rightClick = false;
    }
    public void HandleRotatBuildingInput()
    {
        if(r_input)
        {
            r_input = false;
            // buildingSystem.RotateBuilding();
        }
    }
    public void DisplayCursor()
    {
        ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit, 1000f, clickableLayer) && !EventSystem.current.IsPointerOverGameObject())
        {
            UpdateCursorUI();
        }
        else if (Physics.Raycast(ray, out hit, 1000f, defaultLayer) && !EventSystem.current.IsPointerOverGameObject())
        {
        }
        else if (Physics.Raycast(ray, out hit, 1000f, notClickableLayer) && !EventSystem.current.IsPointerOverGameObject())
        {
            cubeBeingDisplayed?.HighlightTile(false);
            cubeBeingDisplayed = null;
        }
    }
    private void HandleMouseInput()
    {
        if (rightClick) {
            // rightClick=false;
            // uIManager.HideAll();
            // DeselectCube(true);
            // buildingSystem.Deselect();
        }
        if(leftClick) {
            ClickOnThing(hit);
        }
    }
    public void UpdateCursorUI()
    {
        if (hit.transform.CompareTag("Clickable"))
            SelectCube(hit.transform.gameObject.GetComponent<ClickableTile>());

        // if (hit.transform.CompareTag("UI"))
        //     SelectCube(null);

        // else
        // {
        //     // if(cubeBeingDisplayed!=null)
        //     //     CursorUI.SetActive(false);
        //     return;
        // }

        if(cursorMode == CursorMode.Select)
        {
            if (leftClick)
            {

                // Debug.Log($"EventSystem.current.IsPointerOverGameObject() {EventSystem.current.IsPointerOverGameObject()}");
                // if (EventSystem.current.IsPointerOverGameObject())
                // {
                //     Debug.Log($"{hit.transform.gameObject}");
                //     return;
                // }
            }
        }
        else if(cursorMode == CursorMode.Build)
        {
            // if(cubeBeingDisplayed!=null)
            //     buildingSystem.PreviewBuilding(hit);
            // else
            //     buildingSystem.Deselect();

            // if (rightClick)
            // {
            //     cursorMode = CursorMode.Select;
            // }
        }
    }
    public void TurnOffBuildMode()
    {
        cursorMode = CursorMode.Select;
    }
    public void SelectCube(ClickableTile tileOver)
    {
        if(cubeBeingDisplayed != null && cubeBeingDisplayed != tileOver)
        {
            // Debug.Log($"we are not null and not the same cube");
            cubeBeingDisplayed.HighlightTile(false);
        }

        if(cubeBeingDisplayed == tileOver)
            return;

        cubeBeingDisplayed = tileOver;
        cubeBeingDisplayed.HighlightTile(true);
        // gameCursor.UpdateLineRenderer(cubeBeingDisplayed.corners);
        
        // CursorUI.transform.position = Vector3.Lerp(CursorUI.transform.position, cubeBeingDisplayed.gameObject.transform.position, Time.deltaTime * 15f);
    }
    public void DeselectCube(bool playAudio = false)
    {
        // if(cubeSelected!=null)
        // {
        //     cubeSelected.EnableOutline(false);

        //     if(playAudio)
        //         IAudioRequester.instance.PlaySFX("deselectCube");
        // }

        // cubeSelected = null;
        // uIManager.HideTilePopUp();
    }
    public void ClickOnThing(RaycastHit hit)
    {
        if(cubeBeingDisplayed == null)
            return;

        gameboard.MarkVertexAsClicked(cubeBeingDisplayed.gameObject.GetComponent<ClickableTile>());
    }
    private void HandleEscape()
    {
        if(esc_input)
        {
            // if(uIManager.UiIsOpen)
            //     uIManager.UiIsOpen = false;

            // foreach(BiomeHolder bh in playerManager.biomeHolders)
            //     bh.outline.enabled = false;

            esc_input = false;
        }
    }
}
}