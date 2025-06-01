using UnityEngine;

namespace EnerGeoCamera
{
    [RequireComponent(typeof(Camera))]
    public class CameraViewport : MonoBehaviour
    {
    
        [SerializeField] private Camera _camera;
        private void Awake()
        {
            _camera = gameObject.GetComponent<Camera>();
            _camera.rect = new Rect(0.25f, 0f, 0.5f, 1f);
        }
    }
}
