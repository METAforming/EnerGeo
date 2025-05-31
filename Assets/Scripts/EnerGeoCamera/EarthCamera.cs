using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CesiumForUnity;
using Unity.Mathematics;
using System.Linq;

namespace EnerGeoCamera
{
    [RequireComponent(typeof(Camera))]
    public class EarthCamera : MonoBehaviour
    {
        #region Customizable Variables
        [Header("Camera Component")]
        [SerializeField]
        private bool _enableMovement;
        
        public bool EnableMovement
        {
            get => _enableMovement;
            set => _enableMovement = value;
        }
        
        [SerializeField]
        private float _initialViewWidth;
        [SerializeField]
        private float _initialViewHeight;
        
        [Space]
        
        [SerializeField]
        private InputActionProperty _moveAction;
        [SerializeField]
        private InputActionProperty _zoomAction;
        
        [Header("Camera Movement")]
        [SerializeField]
        [Min(0.0f)]
        private float _maxSpeed = 100.0f;
        
        [SerializeField]
        [Min(0.1f)]
        private float _acceleration = 1.0f;
        
        [SerializeField]
        [Min(0.1f)]
        private float _deceleration = 0.1f;
        #endregion
        
        #region Event Functions
        protected void Awake()
        {
            _georeference = GetComponentInParent<CesiumGeoreference>();
            if (_georeference is null)
            {
                #if UNITY_EDITOR
                Debug.LogError("EarthCamera must be nested under a game object with a CesiumGeoreference.");
                #endif 
            }

            _globeAnchor = GetComponentInParent<CesiumGlobeAnchor>();
            if (_globeAnchor is null)
            {
                Debug.LogError("EarthCamera must be nested under a game object with a CesiumGlobeAnchor");
            }

            CesiumOriginShift originShift = _globeAnchor.GetComponent<CesiumOriginShift>();
            if (originShift is null)
            {
                Debug.LogError($"EarthCamera expects a CesiumOriginShift on {_globeAnchor?.name}, none found");
            }
            
            InitializeCamera();
            InitializeController();
            ConfigureInputs();
        }
        #endregion

        #region Intialization
        private void InitializeCamera()
        {
            _camera = gameObject.GetComponent<Camera>();
            _camera.rect = new Rect(0.25f, 0f, 0.5f, 1f);
            _initialViewWidth = _camera.rect.width;
            _initialViewHeight = _camera.rect.height;
        }

        private void InitializeController()
        {
            if (_globeAnchor.GetComponent<CharacterController>() != null)
            {
                _controller = _globeAnchor.GetComponent<CharacterController>();
                _controller.hideFlags = HideFlags.HideInInspector;
            } 
        }
        #endregion
        
        #region Update

        private void FixedUpdate()
        {
            HandlePlayerInput();
        }

        #endregion

        #region Player Inputs
        private void HandlePlayerInput()
        {
            Vector2 moveDelta = _moveAction.action.ReadValue<Vector2>();
            #if UNITY_EDITOR
            Debug.Log(moveDelta);
            #endif 

            float inputUp = moveDelta.y;
            float inputRight = moveDelta.x;

            float zoomIn = _zoomAction.action.ReadValue<float>();

            Vector3 movementInput = new Vector3(inputRight, inputUp, zoomIn);

            if (_enableMovement)
            {
                Move(movementInput);
            }
        }
        
        private void Move(Vector3 movementInput)
        {
            Vector3 inputDirection = transform.right * movementInput.x + transform.forward * movementInput.z;

            // Georeference 클래스가
            // 지구 중심 좌표계(Earth-Center Earth-Fixed, ECEF)에서
            // Unity 좌표계로 변환
            if (_georeference != null)
            {
                double3 positionECEF = _globeAnchor.positionGlobeFixed;
                double3 upECEF = _georeference.ellipsoid.GeodeticSurfaceNormal(positionECEF);
                double3 upUnity = _georeference.TransformEarthCenteredEarthFixedDirectionToUnity(upECEF);

                inputDirection = (float3)inputDirection + (float3)upUnity * movementInput.y;
            }

            if (inputDirection != Vector3.zero)
            {
                if (_velocity.magnitude > 0.0f)
                {
                    Vector3 directionChange = inputDirection - _velocity.normalized;
                    _velocity += directionChange * _velocity.magnitude * Time.fixedDeltaTime;
                    _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
                }
                
                // TODO: calculate velocity based
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

                if (_globeAnchor.detectTransformChanges)
                {
                    _globeAnchor.Sync();
                }
            }
        }
        
        private bool HasInputAction(InputActionProperty property)
        {
            return (property.action != null && property.action.bindings.Any()) || (property.reference is not null);
        }

        private void ConfigureInputs()
        {
            InputActionMap map = new InputActionMap("EarthCamera Controller");

            if (!HasInputAction(_moveAction))
            {
                // 기본적으로 왼손 VR 컨트롤러로 조작
                InputAction newMoveAction = map.AddAction("move", binding: "<XRController>{LeftHand}/thumbstick");
                // 키보드 조작 추가
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
                    .With("Negative", "<Keyboard>/o")
                    .With("Positive", "<Keyboard>/i");
                _zoomAction = new InputActionProperty(newZoomAction);
            }
            
            _moveAction.action.Enable();
            _zoomAction.action.Enable();
        }
        #endregion
        
        private Camera _camera;

        private CharacterController _controller;
        private CesiumGeoreference _georeference;
        private CesiumGlobeAnchor _globeAnchor;

        // private CurveF
        private Vector3 _velocity = Vector3.zero;
        
    }
}
