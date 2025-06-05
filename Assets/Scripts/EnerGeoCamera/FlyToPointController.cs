using System;
using System.Collections.Generic;
using System.Linq;
using CesiumForUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnerGeoCamera
{
    public class FlyToPointController : MonoBehaviour
    {
        [Header("Main Camera")] [SerializeField]
        private CesiumFlyToController _cameraFlyController;

        [Space] [Header("Markers")] public List<EarthMarker> _markers = new List<EarthMarker>();

        /// <summary>
        /// X: Yaw, Y: Pitch
        /// </summary>
        public List<Vector2> _viewAngle = new List<Vector2>();

        private CesiumGeoreference _georeference;
        
        
        private EarthCamera _earthCamera;
        private EarthMarker _currentMarker;
        private int _currentMarkerIndex = -1;
        private double3 _defaultPosition;

        #region Event Functions

        public void Awake()
        {
            _earthCamera = _cameraFlyController.gameObject.GetComponent<EarthCamera>();
            _georeference = GetComponent<CesiumGeoreference>();
            _defaultPosition = _georeference.TransformUnityPositionToEarthCenteredEarthFixed((float3)_earthCamera.transform.position);
        }

        public void Update()
        {
            if (!_markers.Any()) return;

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                _earthCamera.EnableSatelliteView = true;
                _earthCamera.EnableRotation = false;
                
                ChangeCurrentMarker();
                if (_cameraFlyController)
                {
                    FlyToMarkerPosition();
                }
            } else if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                ResetViewPoint();
            }
        }

        private void OnValidate()
        {
            if (_viewAngle.Count > 0 && _viewAngle.Count > _markers.Count)
            {
                _viewAngle.RemoveRange(_markers.Count - 1, _viewAngle.Count - _markers.Count);
            }
        }

        #endregion


        #region Marker Management

        /// <summary>
        /// Change selected marker
        /// </summary>
        private void ChangeCurrentMarker()
        {
            if (_currentMarkerIndex < 0)
            {
                _currentMarkerIndex = 0;
            }
            else
            {
                _currentMarkerIndex++;
                if (_currentMarkerIndex >= _markers.Count) _currentMarkerIndex -= _markers.Count;
            }

            _currentMarker = _markers[_currentMarkerIndex];
        }

        private void FlyToMarkerPosition()
        {
            Vector2 currentViewAngle = _currentMarkerIndex >= _viewAngle.Count
                ? new Vector2(0.0f, 0.0f)
                : _viewAngle[_currentMarkerIndex];

            _cameraFlyController.FlyToLocationLongitudeLatitudeHeight(
                _currentMarker.GetMarkerGeolocation(),
                currentViewAngle.x,
                currentViewAngle.y,
                false);
            
            _earthCamera.EnableSatelliteView = false;
            _earthCamera.EnableRotation = true;
        }

        private void ResetViewPoint()
        {
            _earthCamera.EnableSatelliteView = true;
            _earthCamera.EnableRotation = false;

            // CHORE:
            // Too hard-coded algorithm for returning to default camera view state
            _cameraFlyController.FlyToLocationEarthCenteredEarthFixed(_defaultPosition, 0.0f, 90.0f, false);
        }
        
        //TODO:
        // 1. hide markers when fly to the point
        // 2. calculate the perfect view angle for dynamic positions

        #endregion
    }
}