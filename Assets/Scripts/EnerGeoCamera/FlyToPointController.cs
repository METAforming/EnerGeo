using System;
using System.Collections.Generic;
using System.Linq;
using CesiumForUnity;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace EnerGeoCamera
{
    [Serializable]
    public struct EarthMarker
    {
        public double longitude;
        public double latitude;
        public double height;

        public double3 GetMarkerPosition()
        {
            return new double3(longitude, latitude, height);
        }
    }

    public class FlyToPointController : MonoBehaviour
    {
        [Header("Main Camera")]
        [SerializeField]
        private CesiumFlyToController _cameraFlyController;
        
        [Space]
        
        [Header("Markers")]
        public List<EarthMarker> _markers = new List<EarthMarker>();
        /// <summary>
        /// X: Yaw, Y: Pitch
        /// </summary>
        public List<Vector2> _viewAngle = new List<Vector2>();
        
        private EarthMarker _currentMarker;
        private int _currentMarkerIndex = -1;

        #region Event Functions

        public void Update()
        {
            if (!_markers.Any()) return;

            if (Keyboard.current.tabKey.isPressed)
            {
                Debug.Log("Tab key pressed");
                ChangeCurrentMarker();
                if (_cameraFlyController)
                {
                    FlyToMarkerPosition();
                }
            }
        }

        private void OnValidate()
        {
            if (_markers.Count < _viewAngle.Count)
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
                _currentMarker.GetMarkerPosition(),
                currentViewAngle.x,
                currentViewAngle.y,
                false);
        }

        #endregion
    }
}