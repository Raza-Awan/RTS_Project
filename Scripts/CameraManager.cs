using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class CameraManager : MonoBehaviour
{
    private Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

    public class CameraEvent
    {
        public Vector3 point;
        public BaseItemScript baseItem;
    }

    public enum RaycastTarget
    {
        BASE_ITEM,
        GROUND
    }

    public static CameraManager instance;

    //Objects refs
    public Camera MainCamera;
    public Text camText;

    public EventSystem EventSystem;

    //public events
    public UnityAction<CameraEvent> OnItemTap;
    public UnityAction<CameraEvent> OnItemDragStart;
    public UnityAction<CameraEvent> OnItemDrag;
    public UnityAction<CameraEvent> OnItemDragStop;
    public UnityAction<CameraEvent> OnTapGround;

    //private variables
    private int _layerMaskBaseItemCollider;
    private int _layerMaskGroundCollider;

    private float screenRatio = Screen.width / Screen.height;
    private Vector2 _defaultTouchPos = new Vector2(9999, 9999);
    private float _minimunMoveDistanceForItemMove = 0.2f;
    private float _maxZoomFactor = 25f;
    private float _minZoomFactor = 5f;
    private float _clampZoomOffset = 2f;

    private Vector3 _tapItemStartPos;
    private Vector3 _tapGroundStartPosition;
    private Vector3 initialPosition;

    private bool _isTappedBaseItem;
    private bool _isDraggingBaseItem;
    private bool _isPanningScene;
    private bool _isPanningSceneStarted;

    public BaseItemScript _selectedBaseItem;
    public BuildingGridPlacer buildingPlacer;

    private void Awake()
    {
        instance = this;
        _layerMaskBaseItemCollider = LayerMask.GetMask("BaseItemCollider");
        _layerMaskGroundCollider = LayerMask.GetMask("GroundCollider");
    }

    private void Start()
    {
        initialPosition = MainCamera.transform.position;
    }

    private void Update()
    {
        camText.text = "x = " + MainCamera.transform.position.x + " , " +
                       "y = " + MainCamera.transform.position.y + " , " +
                       "z = " + MainCamera.transform.position.z;


        //if (IsOutsideBounds())
        //{
        //    // Reset the camera position to the initial position or perform any desired action
        //    this.MainCamera.transform.position = Vector3.Lerp(MainCamera.transform.position, initialPosition, Time.deltaTime * 2);
        //}
        if (float.IsNaN(MainCamera.transform.position.x) && float.IsNaN(MainCamera.transform.position.y) && float.IsNaN(MainCamera.transform.position.z))
        {
            MainCamera.transform.position = initialPosition;
        }

        if (IsUsingUI())
        {
            return;
        }

        UpdateBaseItemTap();
        UpdateGroundTap();
        UpdateBaseItemMove();
        UpdateScenePan();
        UpdateSceneZoom();
    }



    //when hovering over any ui 
    private bool IsUsingUI()
    {
        if (_isDraggingBaseItem)
        {
            return false;
        }

        if (_isPanningSceneStarted)
        {
            return false;
        }

        return (EventSystem.IsPointerOverGameObject() || EventSystem.IsPointerOverGameObject(0));
    }

    private void _GetTouches(out int touchCount, out Vector2 touch0, out Vector2 touch1)
    {
        touchCount = 0;
        touch0 = _defaultTouchPos;
        touch1 = _defaultTouchPos;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            touchCount = Input.touchCount;
            touch0 = Input.GetTouch(0).position;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                touchCount = 1;
                touch0 = Input.mousePosition;
            }
            else
            {
                touchCount = 0;
                touch0 = _defaultTouchPos;
            }
        }
    }

    private bool _TryGetRaycastHit(Vector2 touch, out RaycastHit hit, RaycastTarget target)
    {
        Ray ray = MainCamera.ScreenPointToRay(touch);
        return (Physics.Raycast(ray, out hit, 1000, (target == RaycastTarget.BASE_ITEM) ? _layerMaskBaseItemCollider : _layerMaskGroundCollider));
    }

    private BaseItemScript _TryGetRaycastHitBaseItem(Vector2 touch)
    {
        RaycastHit hit;
        if (_TryGetRaycastHit(touch, out hit, RaycastTarget.BASE_ITEM))
        {
            return hit.collider.gameObject.GetComponent<BaseItemScript>();
        }

        return null;
    }

    private Vector3 _TryGetRaycastHitBaseGround(Vector2 touch)
    {
        RaycastHit hit;
        if (_TryGetRaycastHit(touch, out hit, RaycastTarget.GROUND))
        {
            return hit.point;
        }

        return positiveInfinityVector;
    }
    
    /// <summary>
    /// TAP ON ITEM //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    //when we tap on any item
    private void UpdateBaseItemTap()
    {
        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        if (_isPanningSceneStarted)
        {
            return;
        }

        if (_isDraggingBaseItem)
        {
            return;
        }

        if (IsUsingUI())
        {
            return;
        }

        BaseItemScript baseItemTapped = _TryGetRaycastHitBaseItem(Input.mousePosition);
        if (baseItemTapped != null)
        {
            _isTappedBaseItem = true;

            _selectedBaseItem = baseItemTapped;
            buildingPlacer._buildingPrefab = _selectedBaseItem.gameObject;
            buildingPlacer._toBuild = buildingPlacer._buildingPrefab;
            buildingPlacer._EnableGridVisual(true);
            BuildingManager m = buildingPlacer._toBuild.GetComponent<BuildingManager>();
            m.lastValidPos = buildingPlacer._buildingPrefab.transform.position;

            BuildingOptionsUI.instance.ShowInfoUI();
            BuildingOptionsUI.instance.ShowUpgradeUI();
            BuildingOptionsUI.instance.ShowTrainUI();
            BuildingOptionsUI.instance.ShowRemoveUI();


            CameraEvent evt = new CameraEvent()
            {
                baseItem = baseItemTapped
            };
            if (OnItemTap != null)
            {
                OnItemTap.Invoke(evt);
            }
        }
        else
        {
            _isTappedBaseItem = false;
            _selectedBaseItem = null;

            if (buildingPlacer._buildingPrefab != null)
            {
                BuildingOptionsUI.instance.HideInfoUI();
                BuildingOptionsUI.instance.HideUpgradeUI();
                BuildingOptionsUI.instance.HideTrainUI();
                BuildingOptionsUI.instance.HideRemoveUI();

                buildingPlacer._EnableGridVisual(false);
                BuildingManager m = buildingPlacer._toBuild.GetComponent<BuildingManager>();
                m.arrowsUI.SetActive(false);
                m.ResetToValidPosition();
                buildingPlacer._buildingPrefab = null;
                buildingPlacer._toBuild = null;
            }
        }
    }

    /// <summary>
    /// TAP ON THE GROUND //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    //when we tap on ground
    private void UpdateGroundTap()
    {
        if (_isTappedBaseItem)
        {
            return;
        }

        if (_isDraggingBaseItem)
        {
            return;
        }

        if (_isPanningScene)
        {
            return;
        }

        if (_isPanningSceneStarted)
        {
            return;
        }

        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        Vector3 tapPosition = _TryGetRaycastHitBaseGround(Input.mousePosition);
        if (tapPosition != positiveInfinityVector)
        {
            CameraEvent evt = new CameraEvent()
            {
                point = tapPosition
            };
            if (OnTapGround != null)
            {
                OnTapGround.Invoke(evt);
            }
        }
    }

    /// <summary>
    /// MOVE / DRAG THE ITEM //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private BaseItemScript _tapStartRaycastItem = null;
    [HideInInspector] public  bool _isDragItemStarted;
    private bool _baseItemMoved;

    //when we are moving any item
    private void UpdateBaseItemMove()
    {
        // On Click
        if (Input.GetMouseButtonDown(0))
        {
            _tapItemStartPos = _TryGetRaycastHitBaseGround(Input.mousePosition);       //checking position where we have tapped on ground
            _tapStartRaycastItem = _TryGetRaycastHitBaseItem(Input.mousePosition);  //checking on which item we have tapped on
            _isDraggingBaseItem = false;    //because we don't want to drag on mouse click but on mouse drag
            _isDragItemStarted = false;       //because on mouse click we have not started dragging yet
        }

        // On Drag started
        if (Input.GetMouseButton(0) && _tapItemStartPos != positiveInfinityVector)
        {
            if (_isTappedBaseItem && _selectedBaseItem == _tapStartRaycastItem)
            {
                Vector3 currentTapPosition = _TryGetRaycastHitBaseGround(Input.mousePosition);
                if (Vector3.Distance(_tapItemStartPos, currentTapPosition) >= _minimunMoveDistanceForItemMove)
                {
                    CameraEvent evt = new CameraEvent()
                    {
                        point = currentTapPosition,
                        baseItem = _selectedBaseItem
                    };

                    if (!_isDragItemStarted)
                    {
                        _isDragItemStarted = true;
                        if (OnItemDragStart != null)
                        {
                            OnItemDragStart.Invoke(evt);
                        }
                    }

                    _isDraggingBaseItem = true;
                    if (OnItemDrag != null)
                    {
                        OnItemDrag.Invoke(evt);
                    }

                    
                    buildingPlacer.PlaceSelectedBuilding();
                }
            }
        }

        // On Drag Ended
        if (Input.GetMouseButtonUp(0))
        {
            _tapItemStartPos = positiveInfinityVector;
            if (_isDragItemStarted)
            {
                _isDragItemStarted = false;
                _isDraggingBaseItem = false;
                if (OnItemDragStop != null)
                {
                    OnItemDragStop.Invoke(null);
                }
            }
        }
    }

    /// <summary>
    /// PAN ON SCREEN //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private int _previousTouchCount = 0;
    private bool _touchCountChanged;
    private Vector2 _touchPosition;
    private bool _canPan;
    private Vector3 _previousPanPoint;
    private Vector3 _panVelocity = Vector3.zero;

    private void _RefreshTouchValues()
    {
        this._touchCountChanged = false;
        int touchCount = 0;
        bool isInEditor = false;


        if (Input.touchCount == 0)
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
            {
                //editor
                touchCount = 1;
                isInEditor = true;
            }
            else
            {
                touchCount = 0;
            }

        }
        else
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touchCount = 0;
            }
            else
            {
                touchCount = Input.touchCount;
            }
        }

        if (touchCount != this._previousTouchCount)
        {
            if (touchCount != 0)
            {
                this._touchCountChanged = true;
            }
        }

        if (isInEditor)
        {
            this._touchPosition = (Vector2)Input.mousePosition;
        }
        else
        {
            if (touchCount == 1)
            {
                this._touchPosition = Input.GetTouch(0).position;
            }
            else if (touchCount >= 2)
            {
                this._touchPosition = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2.0f;
            }
        }

        this._canPan = (touchCount > 0);

        this._previousTouchCount = touchCount;
    }

    public void OnChangeTouchCountScenePan(CameraEvent evt)
    {
        this._previousPanPoint = evt.point;
    }

    public void OnScenePan(CameraEvent evt)
    {
        Vector3 delta = this._previousPanPoint - evt.point;

        if (float.IsNaN(delta.x) && float.IsNaN(delta.y) && float.IsNaN(delta.z))
        {
            return;
        }

        this.MainCamera.transform.localPosition += delta;
        this._panVelocity = delta;

        this.ClampCamera();
    }

    public void OnStopScenePan(CameraEvent evt)
    {
        //		Debug.Log ("OnStopPan");
    }

    public void UpdatePanInertia()
    {
        if (float.IsNaN(_panVelocity.x) && float.IsNaN(_panVelocity.y) && float.IsNaN(_panVelocity.z))
        {
            return;
        }
        if (this._panVelocity.magnitude < 0.05f)
        {
            this._panVelocity = Vector3.zero;
        }
        if (this._panVelocity != Vector3.zero)
        {
            this._panVelocity = Vector3.Lerp(_panVelocity, Vector3.zero, Time.deltaTime * 2);
            this.MainCamera.transform.localPosition += this._panVelocity;
            this.ClampCamera();
        }
    }

    //clamps the camera within the scene limits, the limits can adjusted with '_CameraClampLeft' and 
    //'_CameraClampRight' components
    public void ClampCamera()
    {
        //		return;
        float worldSizePerPixel = 2 * this.MainCamera.orthographicSize / (float)Screen.height;

        //clamp camera left and top
        Vector3 leftClampScreenPos = this.MainCamera.WorldToScreenPoint(CameraBoundScript.instance.CameraClampTopLeftPosition);
        //Left
        if (leftClampScreenPos.x > 0)
        {
            float deltaFactor = leftClampScreenPos.x * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, 0);
            delta = this.MainCamera.transform.TransformVector(delta);
            this.MainCamera.transform.localPosition += delta;
        }
        //Top
        if (leftClampScreenPos.y < Screen.height)
        {
            float deltaFactor = (Screen.height - leftClampScreenPos.y) * worldSizePerPixel;
            Vector3 delta = new Vector3(-deltaFactor, 0, -deltaFactor);
            this.MainCamera.transform.localPosition += delta;
        }

        //clamp camera right and bottom
        Vector3 rightClampScreenPos = this.MainCamera.WorldToScreenPoint(CameraBoundScript.instance.CameraClampBottomRightPosition);
        //Right
        if (rightClampScreenPos.x < Screen.width)
        {
            float deltaFactor = (rightClampScreenPos.x - Screen.width) * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, 0);
            delta = this.MainCamera.transform.TransformVector(delta);
            this.MainCamera.transform.localPosition += delta;
        }
        //Bottom
        if (rightClampScreenPos.y > 0)
        {
            float deltaFactor = rightClampScreenPos.y * worldSizePerPixel;
            Vector3 delta = new Vector3(deltaFactor, 0, deltaFactor);
            this.MainCamera.transform.localPosition += delta;
        }
    }

    bool IsOutsideBounds()
    {
        // Get the position and rotation of the camera
        Vector3 cameraPosition = this.MainCamera.transform.position;
        Quaternion cameraRotation = this.MainCamera.transform.rotation;

        // Calculate the forward direction from the local Z-axis of the camera
        Vector3 rayDirection = this.MainCamera.transform.forward;

        // Fire a ray from the camera position in the forward direction
        Ray ray = new Ray(cameraPosition, rayDirection);
        RaycastHit hit;

        // Set the raycast distance based on your ground's height or other considerations
        float raycastDistance = Mathf.Infinity;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, raycastDistance, LayerMask.GetMask("Ground")))
        {
            // Ground layer hit, camera is inside bounds
            return false;
        }
        else
        {
            // No ground layer hit, camera is outside bounds
            return true;
        }
    }



    public void UpdateScenePan()
    {
        this._RefreshTouchValues();

        if (this._isDraggingBaseItem)
        {
            return;
        }

        if (this._touchCountChanged)
        {
            this._tapGroundStartPosition = this._TryGetRaycastHitBaseGround(this._touchPosition);
        }

        if (this._canPan)
        {
            Vector3 currentTapPosition = this._TryGetRaycastHitBaseGround(this._touchPosition);

            if (this._touchCountChanged)
            {
                CameraEvent evt = new CameraEvent()
                {
                    point = currentTapPosition
                };
                this.OnChangeTouchCountScenePan(evt);
            }

            if (!this._isPanningSceneStarted && Vector3.Distance(this._tapGroundStartPosition, currentTapPosition) >= 1f)
            {
                this._isPanningSceneStarted = true;
                this._previousPanPoint = currentTapPosition;
            }

            if (this._isPanningSceneStarted)
            {
                CameraEvent evt = new CameraEvent()
                {
                    point = currentTapPosition
                };

                this._isPanningScene = true;
                this.OnScenePan(evt);
            }

        }
        else
        {
            this._isPanningScene = false;

            if (this._isPanningSceneStarted)
            {
                this._isPanningSceneStarted = false;
                this.OnStopScenePan(null);
            }
        }

        if (!this._isPanningScene)
        {
            this.UpdatePanInertia();
        }
    }

    /// <summary>
    /// ZOOM ON SCREEN //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    private Vector3 _touchPoint1;
    private Vector3 _touchPoint2;
    private bool _isZoomingStarted;
    private float _previousPinchDistance;
    private float _oldZoom = -1;

    //when we are zooming the scene
    private void UpdateSceneZoom()
    {
        if (_isDraggingBaseItem)
        {
            return;
        }

        float newZoom = MainCamera.orthographicSize;

        //In Editor
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        if (scrollAmount != 0)
        {
            newZoom = newZoom - scrollAmount;
        }

        //On Touch Device (Andriod)
        if (Input.touchCount == 0)
        {
            _isZoomingStarted = false;
        }

        if (Input.touchCount == 2)
        {
            _touchPoint1 = _TryGetRaycastHitBaseGround(Input.GetTouch(0).position);
            _touchPoint2 = _TryGetRaycastHitBaseGround(Input.GetTouch(1).position);
            if (!_isZoomingStarted)
            {
                _isZoomingStarted = true;
                _previousPinchDistance = (_touchPoint2 - _touchPoint1).magnitude;
            }
        }

        if (_isZoomingStarted)
        {
            float _currentPinchDistance = (_touchPoint2 - _touchPoint1).magnitude;
            float delta = _previousPinchDistance - _currentPinchDistance;
            newZoom = MainCamera.orthographicSize + (delta / (2 * screenRatio));
        }

        //clamp zoom 
        newZoom = Mathf.Clamp(newZoom - scrollAmount, _minZoomFactor, _maxZoomFactor);
        if (newZoom < _minZoomFactor + _clampZoomOffset)
        {
            newZoom = Mathf.Lerp(newZoom, _minZoomFactor + _clampZoomOffset, Time.deltaTime * 2);
        }
        else if (newZoom > _maxZoomFactor - _clampZoomOffset)
        {
            newZoom = Mathf.Lerp(newZoom, _maxZoomFactor - _clampZoomOffset, Time.deltaTime * 2);
        }

        if (_oldZoom != newZoom)
        {
            MainCamera.orthographicSize = newZoom;
            ClampCamera();
            _oldZoom = newZoom;
        }
    }
}
