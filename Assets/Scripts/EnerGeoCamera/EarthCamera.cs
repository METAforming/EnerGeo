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
        [SerializeField]
        private bool _enableMovement;
        #endregion
        
        protected void Awake()
        {
            InitializeCamera();
        }

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

        private void HandlePlayerInput()
        {
            Vector2 moveDelta = _moveAction.action.ReadValue<Vector2>();

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
            }
            else
            {
                float speed = Mathf.Max(_velocity.magnitude, 0.0f);
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
                InputAction newMoveAction = map.AddAction("move", binding: "<Gamepad>/leftStick");
                newMoveAction.AddCompositeBinding("Dpad")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
                _moveAction = new InputActionProperty(newMoveAction);
            }
        }

        private Camera _camera;
        private float _initialViewWidth;
        private float _initialViewHeight;

        private CharacterController _controller;
        private CesiumGeoreference _georeference;
        private CesiumGlobeAnchor _globeAnchor;

        private Vector3 _velocity = Vector3.zero;
        private float _maxSpeed = 100.0f;
        
        private InputActionProperty _moveAction;
        private InputActionProperty _zoomAction;

        public bool EnableMovement
        {
            get => _enableMovement;
            set
            {
                _enableMovement = value;
            }
        }
    }
}
