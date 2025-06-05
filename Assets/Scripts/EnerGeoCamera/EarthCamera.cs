using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using CesiumForUnity;

namespace EnerGeoCamera
{
    [RequireComponent(typeof(Camera))]
    public class EarthCamera : MonoBehaviour
    {
        #region Customizable Variables

        [Header("Camera Component")] 
        [SerializeField]
        private bool _enableSatelliteView;

        public bool EnableSatelliteView
        {
            get => _enableSatelliteView;
            set => _enableSatelliteView = value;
        }

        [SerializeField] [Range(0.1f, 1.0f)] 
        private float _initialViewWidth = 1.0f;

        [SerializeField] [Range(0.1f, 1.0f)] 
        private float _initialViewHeight = 1.0f;

        [Space] 
        [Header("Camera Movement")] 
        [SerializeField]
        private bool _enableMovement;

        public bool EnableMovement
        {
            get => _enableMovement;
            set => _enableMovement = value;
        }

        [SerializeField] private bool _enableRotation;

        public bool EnableRotation
        {
            get => _enableRotation;
            set
            {
                ResetSpeed();
                _enableRotation = value;
            }
        }
        
        [SerializeField] private bool _enableDynamicSpeed;
        public bool EnableDynamicSpeed
        {
            get => _enableDynamicSpeed;
            set => _enableDynamicSpeed = value;
        }
        
        [SerializeField] private bool _enableDynamicClippingPlanes;
        public bool EnableDynamicClippingPlanes
        {
            get => _enableDynamicClippingPlanes;
            set => _enableDynamicClippingPlanes = value;
        }
      

        [SerializeField] [Min(0.0f)] private float _defaultMaximumSpeed;

        [SerializeField] [Min(0.0f)] private float _dynamicSpeedMinHeight;
        
        [SerializeField] [Min(0.0f)] private float _lookSpeed;

        [Space]
        
        [Header("Player Inputs")]
        
        [SerializeField]
        private InputActionProperty _moveAction;

        [SerializeField] private InputActionProperty _zoomAction;

        [SerializeField] private InputActionProperty _lookAction;

        [SerializeField] private InputActionProperty _toggleDynamicSpeedAction;

        [SerializeField] private InputActionProperty _toggleCameraViewState;

        [SerializeField] private InputActionProperty _speedResetAction;

        [SerializeField] private InputActionProperty _speedChangeAction;

        #endregion

        #region Private Variables

        private Camera _camera;
        private float _initialNearClipPlane;
        private float _initialFarClipPlane;

        // Essential components for transforming between Cesium coordinate system and Unity coordinate system
        private CharacterController _controller;
        private CesiumGeoreference _georeference;
        private CesiumGlobeAnchor _globeAnchor;

        // Camera movement related variables
        private Vector3 _velocity = Vector3.zero;
        private float _acceleration = 20000.0f;
        private float _deceleration = 9999999959.0f;

        private float _maxSpeed = 100.0f;
        private float _maxSpeedPreMultiplier = 0.0f;
        private float _speedMultiplier = 1.0f;
        private float _speedMultiplierIncrement = 1.5f;
        private AnimationCurve _maxSpeedCurve;

        // Hard-coded values for adjusting clipping plane size dynamically
        private float _dynamicClippingPlanesMinHeight = 10000.0f;
        private float _maximumFarClipPlane = 500000000.0f;
        private float _maximumNearToFarRatio = 100000.0f;
        private float _maximumNearClipPlane = 1000.0f;

        private float _maxRaycastDistance = 1000000.0f;

        #endregion

        #region Event Functions

        protected void Awake()
        {
            _georeference = GetComponentInParent<CesiumGeoreference>();

            if (_georeference is null)
            {
                Debug.LogError("EarthCamera must be nested under a game object with a CesiumGeoreference.");
                return;
            }

            _globeAnchor = GetComponentInParent<CesiumGlobeAnchor>();
            if (_globeAnchor is null)
            {
                Debug.LogError("EarthCamera must be nested under a game object with a CesiumGlobeAnchor");
                return;
            }

            CesiumOriginShift originShift = _globeAnchor.GetComponent<CesiumOriginShift>();
            if (originShift is null)
            {
                Debug.LogError($"EarthCamera expects a CesiumOriginShift on {_globeAnchor?.name}, none found");
                return;
            }

            InitializeCamera();
            InitializeController();
            CreateMaxSpeedCurve();
            ConfigureInputs();
        }

        private void Reset()
        {
            CesiumGlobeAnchor anchor = GetComponentInParent<CesiumGlobeAnchor>() ??
                                       gameObject.AddComponent<CesiumGlobeAnchor>();
            CesiumOriginShift origin = anchor.GetComponent<CesiumOriginShift>() ??
                                       anchor.gameObject.AddComponent<CesiumOriginShift>();
        }

        private void FixedUpdate()
        {
            HandlePlayerInputs();

            if (_enableDynamicClippingPlanes)
            {
                UpdateClippingPlanes();
            }
        }

        private void Update()
        {
            // TODO: Delete this block when finished implementing `performed` input function
            if (Keyboard.current.vKey.wasPressedThisFrame)
            {
                _enableSatelliteView = !_enableSatelliteView;
                _enableRotation = !_enableRotation;
            }
        }

        #endregion

        #region Intialization

        private void InitializeCamera()
        {
            _camera = gameObject.GetComponent<Camera>();
            _camera.rect = new Rect(0f, 0f, _initialViewWidth, _initialViewHeight);

            // Adjust camera view frustum
            _initialNearClipPlane = _camera.nearClipPlane;
            _initialFarClipPlane = _camera.farClipPlane;
        }

        private void InitializeController()
        {
            if (_globeAnchor.GetComponent<CharacterController>() != null)
            {
                _controller = _globeAnchor.GetComponent<CharacterController>();
            }
            else
            {
                _controller = _globeAnchor.gameObject.AddComponent<CharacterController>();
                _controller.hideFlags = HideFlags.HideInInspector;
            }
        }

        private void CreateMaxSpeedCurve()
        {
            // This creates a curve that is linear between the first two keys,
            // then smoothly interpolated between the last two keys.
            Keyframe[] keyframes =
            {
                new Keyframe(0.0f, 4.0f),
                new Keyframe(10000000.0f, 10000000.0f),
                new Keyframe(13000000.0f, 2000000.0f)
            };

            keyframes[0].weightedMode = WeightedMode.Out;
            keyframes[0].outTangent = keyframes[1].value / keyframes[0].value;
            keyframes[0].outWeight = 0.0f;

            keyframes[1].weightedMode = WeightedMode.In;
            keyframes[1].inWeight = 0.0f;
            keyframes[1].inTangent = keyframes[1].value / keyframes[0].value;
            keyframes[1].outTangent = 0.0f;

            keyframes[2].inTangent = 0.0f;

            _maxSpeedCurve = new AnimationCurve(keyframes)
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever
            };
        }

        #endregion

        #region Player Inputs

        private void HandlePlayerInputs()
        {
            // Handle keyboard inputs for moving camera
            Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();
            float inputRight = moveInput.x;
            float inputUp = moveInput.y;

            // Handle keyboard inputs for zoom in and out
            float zoomIn = _zoomAction.action.ReadValue<float>();
            Vector3 movementInput = new Vector3(inputRight, inputUp, zoomIn);

            // Handle mouse inputs for looking around 
            Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();
            float inputHorizontal = lookInput.x;
            float inputVertical = lookInput.y;
            
            bool toggleCameraViewState = _toggleCameraViewState.action.ReadValue<float>() > 0.5f;

            // Handle keyboard inputs for managing camera movement speed
            bool toggleDynamicSpeed = _toggleDynamicSpeedAction.action.ReadValue<float>() > 0.5f;
            bool inputSpeedReset = _speedResetAction.action.ReadValue<float>() > 0.5f;
            float inputSpeedChange = _speedChangeAction.action.ReadValue<Vector2>().y;

            if (_enableMovement)
            {
                Move(movementInput);
            }

            if (_enableRotation)
            {
                Rotate(inputHorizontal, inputVertical);
            }

            if (toggleDynamicSpeed)
            {
                _enableDynamicSpeed = !_enableDynamicSpeed;
            }

            // FIXME:
            // Uncomment this block when finished implementing `performed` input function

            // if (toggleCameraViewState)
            // {
            //     Debug.Log("Toggle Camera view");
            //     _enableSatelliteView = !_enableSatelliteView;
            //     _enableRotation = !_enableRotation;
            // }

            if (inputSpeedReset || (_enableDynamicSpeed && movementInput == Vector3.zero))
            {
                // Set camera's movement speed as the default value
                // when Player wants to reset the speed
                // or stay in a point without any movement inputs
                ResetSpeedMultiplier();
            }
            else
            {
                HandleSpeedChange(inputSpeedChange);
            }
        }

        /// <summary>
        /// Readjust Main Camera's movement speed
        /// </summary>
        /// <param name="speedChangeInput">How much increase/reduce speed</param>
        private void HandleSpeedChange(float speedChangeInput)
        {
            if (_enableDynamicSpeed)
            {
                UpdateDynamicSpeed();
            }
            else
            {
                SetMaxSpeed(_defaultMaximumSpeed);
            }

            if (speedChangeInput == 0.0f) return;

            if (speedChangeInput > 0.0f)
            {
                _speedMultiplier *= _speedMultiplierIncrement;
            }
            else
            {
                _speedMultiplier /= _speedMultiplierIncrement;
            }

            float max = _enableDynamicSpeed ? 50.0f : 50000.0f;
            _speedMultiplier = Mathf.Clamp(_speedMultiplier, 0.1f, max);
        }

        /// <summary>
        /// Rotate Main Camera based on player's mouse inputs
        /// </summary>
        /// <param name="horizontalRotation">How much rotate the camera horizontally(Left, Right)</param>
        /// <param name="verticalRotation">How much rotate the camera vertically(Up, Down)</param>
        private void Rotate(float horizontalRotation, float verticalRotation)
        {
            if (horizontalRotation == 0.0f && verticalRotation == 0.0f)
            {
                return;
            }

            float valueX = verticalRotation * _lookSpeed * Time.fixedDeltaTime;
            float valueY = horizontalRotation * _lookSpeed * Time.fixedDeltaTime;

            // Rotation around the X-axis occurs counter-clockwise, so the look range
            // maps to [270, 360] degrees for the upper quarter-sphere of motion, and
            // [0, 90] degrees for the lower. Euler angles only work with positive values,
            // so map the [0, 90] range to [360, 450] so the entire range is [270, 450].
            // This makes it easy to clamp the values.
            float rotationX = transform.localEulerAngles.x;
            if (rotationX <= 90.0f)
            {
                rotationX += 360.0f;
            }

            float newRotationX = Mathf.Clamp(rotationX - valueX, 270.0f, 450.0f);
            float newRotationY = transform.localEulerAngles.y + valueY;
            transform.localRotation =
                Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);
        }

        private void Move(Vector3 movementInput)
        {
            Vector3 inputDirection = _enableSatelliteView
                ? transform.right * movementInput.x + transform.up * movementInput.y
                : transform.right * movementInput.x + transform.forward * movementInput.y;

            // Georeference 클래스가
            // 지구 중심 좌표계(Earth-Center Earth-Fixed, ECEF)에서
            // Unity 좌표계로 변환
            if (_georeference)
            {
                double3 positionECEF = _globeAnchor.positionGlobeFixed;
                double3 upECEF = _georeference.ellipsoid.GeodeticSurfaceNormal(positionECEF);
                double3 upUnity = _georeference.TransformEarthCenteredEarthFixedDirectionToUnity(upECEF);

                inputDirection = _enableSatelliteView
                    ? (float3)inputDirection + (float3)upUnity * -movementInput.z
                    : (float3)inputDirection + (float3)upUnity * movementInput.z;
            }

            if (inputDirection != Vector3.zero)
            {
                if (_velocity.magnitude > 0.0f)
                {
                    Vector3 directionChange = inputDirection - _velocity.normalized;
                    _velocity += directionChange * _velocity.magnitude * Time.fixedDeltaTime;
                }

                _velocity += inputDirection * _acceleration * Time.fixedDeltaTime;
                _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
            }
            else
            {
                float speed = Mathf.Max(_velocity.magnitude - _deceleration, 0.0f);
                _velocity = Vector3.ClampMagnitude(_velocity, speed);
            }

            if (_velocity != Vector3.zero)
            {
                _controller.Move(_velocity * Time.fixedDeltaTime);

                if (!_globeAnchor.detectTransformChanges)
                {
                    _globeAnchor.Sync();
                }
            }
        }

        /// <summary>
        /// Assign new InputActionProperty for Input Actions that not assigned in inspector
        /// </summary>
        /// <param name="property">Inspection Target Property</param>
        /// <returns>Does Input Action properties set in the inspector</returns>
        private bool HasInputAction(InputActionProperty property)
        {
            return (property.action != null && property.action.bindings.Any()) || (property.reference is not null);
        }

        private void ConfigureInputs()
        {
            InputActionMap map = new InputActionMap("EarthCamera Controller");

            if (!HasInputAction(_moveAction))
            {
                // Basically control using left hand xr controller 
                InputAction newMoveAction = map.AddAction("move", binding: "<XRController>{LeftHand}/thumbstick");
                // Add keyboard inputs
                newMoveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
                _moveAction = new InputActionProperty(newMoveAction);
            }

            if (!HasInputAction(_zoomAction))
            {
                InputAction newZoomAction = map.AddAction("zoom", binding: "<XRController>{RightHand}/thumbstick");
                newZoomAction.AddCompositeBinding("Axis")
                    .With("Negative", "<Keyboard>/q")
                    .With("Positive", "<Keyboard>/e");
                _zoomAction = new InputActionProperty(newZoomAction);
            }

            if (!HasInputAction(_lookAction))
            {
                // TODO:
                // Can't use HMD in InputAction for rotating view
                // Use XRHMD.current for future XR Devices
                InputAction newLookAction = map.AddAction("look", binding: "<Mouse>/delta");
                _lookAction = new InputActionProperty(newLookAction);
            }

            if (!HasInputAction(_toggleDynamicSpeedAction))
            {
                InputAction newToggleAction = map.AddAction("toggleDynamicSpeed", binding: "<Keyboard>/t");
                _toggleDynamicSpeedAction = new InputActionProperty(newToggleAction);
            }

            if (!HasInputAction(_speedChangeAction))
            {
                InputAction newSpeedChangeAction = map.AddAction("speedChange", binding: "<Mouse>/scroll");
                _speedChangeAction = new InputActionProperty(newSpeedChangeAction);
            }

            if (!HasInputAction(_speedResetAction))
            {
                InputAction newSpeedResetAction = map.AddAction("speedReset", binding: "<Keyboard>/n");
                _speedResetAction = new InputActionProperty(newSpeedResetAction);
            }

            // TODO:
            // Implementing custom `performed` function for invoking input functions per click needed
            
            // if (!HasInputAction(_toggleCameraViewState))
            // {
            //     InputAction newToggleCameraViewState = map.AddAction("toggleCameraViewState", binding: "<keyboard>/v");
            //     _toggleCameraViewState = new InputActionProperty(newToggleCameraViewState);
            // }

            _moveAction.action.Enable();
            _zoomAction.action.Enable();
            _lookAction.action.Enable();
            _toggleDynamicSpeedAction.action.Enable();
            // _toggleCameraViewState.action.Enable();
            _speedChangeAction.action.Enable();
            _speedResetAction.action.Enable();
        }

        #endregion

        #region Dynamic Camera Movement

        /// <summary>
        /// Cast the line ray from the Main Camera to the center of the Earth
        /// </summary>
        /// <param name="hitDistance">The distance between Main Camera and the center of the Earth</param>
        /// <returns>Does ray hit the surface of the Earth?</returns>
        private bool RaycastTowardsEarthCenter(out float hitDistance)
        {
            if (_georeference is null)
            {
                hitDistance = 0.0f;
                return false;
            }

            double3 center = _georeference.TransformEarthCenteredEarthFixedPositionToUnity(new double3(0.0));

            RaycastHit hitInfo;
            if (Physics.Linecast(transform.position, (float3)center, out hitInfo))
            {
                hitDistance = Vector3.Distance(transform.position, hitInfo.point);
                return true;
            }

            hitDistance = 0.0f;
            return false;
        }

        private bool RaycastTowardsForward(float rayLength, out float hitDistance)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo, rayLength))
            {
                hitDistance = Vector3.Distance(transform.position, hitInfo.point);
                return true;
            }

            hitDistance = 0.0f;
            return false;
        }

        /// <summary>
        /// Calculate the speed of the Main Camera based on the distance between the Main Camera
        /// and the center of the Earth, and the distance between the Main Camera and objects
        /// along the forward vector 
        /// </summary>
        /// <param name="overrideSpeed">Should override the movement speed?</param>
        /// <param name="newSpeed">Calculated speed for camera controller</param>
        /// <returns>Whether successfully speed calculated</returns>
        private bool GetDynamicSpeed(out bool overrideSpeed, out float newSpeed)
        {
            if (_georeference is null)
            {
                overrideSpeed = false;
                newSpeed = 0.0f;

                return false;
            }

            float height;
            // Raycast from the camera to the center of the Earth and compute the distance.
            // Ignore the result if the height is approximately 0.
            if (!RaycastTowardsEarthCenter(out height) || height < 0.000001f)
            {
                overrideSpeed = false;
                newSpeed = 0.0f;

                return false;
            }

            // Ignore the result of the speed when it increased/decreased too much
            // It would make issues that Camera pass the 3D tiles when loading in the scene
            if (_maxSpeedPreMultiplier > 0.5f)
            {
                float heightToMaxSpeedRatio = height / _maxSpeedPreMultiplier;

                if (heightToMaxSpeedRatio > 1000.0f || heightToMaxSpeedRatio < 0.01f)
                {
                    overrideSpeed = false;
                    newSpeed = 0.0f;

                    return false;
                }
            }

            // Raycast along the camera's view (forward) vector.
            float rayLength =
                Mathf.Clamp(_maxSpeed * 3.0f, 0.0f, _maxRaycastDistance);

            float viewDistance;
            // If the raycast does not hit, then only override speed if the height
            // is lower than the maximum threshold. Otherwise, if both raycasts hit,
            // always override the speed.
            if (!RaycastTowardsForward(rayLength, out viewDistance) || viewDistance < 0.000001f)
            {
                overrideSpeed = height <= _dynamicSpeedMinHeight;
            }
            else
            {
                overrideSpeed = true;
            }

            // Set the speed to be the height of the camera from the center of the Earth
            newSpeed = height;

            return true;
        }

        private void ResetSpeedMultiplier()
        {
            _speedMultiplier = 1.0f;
        }

        private void SetMaxSpeed(float speed)
        {
            float actualSpeed = _maxSpeedCurve.Evaluate(speed);
            _maxSpeed = _speedMultiplier * actualSpeed;
            _acceleration =
                Mathf.Clamp(_maxSpeed * 5.0f, 20000.0f, 10000000.0f);
        }

        private void UpdateDynamicSpeed()
        {
            bool overrideSpeed;
            float newSpeed;
            if (GetDynamicSpeed(out overrideSpeed, out newSpeed))
            {
                if (overrideSpeed || newSpeed >= _maxSpeedPreMultiplier)
                {
                    _maxSpeedPreMultiplier = newSpeed;
                }
            }

            SetMaxSpeed(_maxSpeedPreMultiplier);
        }

        private void ResetSpeed()
        {
            _maxSpeed = _defaultMaximumSpeed;
            _maxSpeedPreMultiplier = 0.0f;
            ResetSpeedMultiplier();
        }

        private void UpdateClippingPlanes()
        {
            if (_camera is null)
            {
                return;
            }

            // Raycast from the camera to the center of the Earth and compute the distance.
            float height = 0.0f;
            if (!RaycastTowardsEarthCenter(out height))
            {
                return;
            }

            float nearClipPlane = _initialNearClipPlane;
            float farClipPlane = _initialFarClipPlane;

            if (height >= _dynamicClippingPlanesMinHeight)
            {
                farClipPlane = height + (float)(2.0 * _georeference.ellipsoid.GetMaximumRadius());
                farClipPlane = Mathf.Min(farClipPlane, _maximumFarClipPlane);

                float farClipRatio = farClipPlane / _maximumNearToFarRatio;

                if (farClipRatio > nearClipPlane)
                {
                    nearClipPlane = Mathf.Min(farClipRatio, _maximumNearClipPlane);
                }
            }

            _camera.nearClipPlane = nearClipPlane;
            _camera.farClipPlane = farClipPlane;
        }

        #endregion
    }
}