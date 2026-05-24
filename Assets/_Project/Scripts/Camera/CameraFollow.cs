using UnityEngine;

class CameraFollow : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new(0, 15, -8);
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private bool lookAtTarget = false;

    [Header("Zoom (Orthographic)")]
    [SerializeField] private float zoomSensitivity = 0.1f;
    [SerializeField] private float minOrthoSize = 4f;
    [SerializeField] private float maxOrthoSize = 14f;
    [SerializeField] private float zoomSmoothTime = 0.1f;

    private Camera _camera;
    private InputSystem_Actions _inputActions;
    private Vector3 _velocityRef = Vector3.zero;
    private float _currentOrthoSize;
    private float _smoothedOrthoSize;
    private float _orthoVelocity = 0;

    void Awake() {
        _camera = GetComponent<Camera>();
        _inputActions = new InputSystem_Actions();
        _currentOrthoSize = _camera.orthographicSize;
        _smoothedOrthoSize = _camera.orthographicSize;
    }

    void OnEnable() {
        _inputActions.Player.Enable();
    }

    void Update() {
        HandleZoom();
    }

    void LateUpdate() {
        if (player == null) return;

        _smoothedOrthoSize = Mathf.SmoothDamp(
            _smoothedOrthoSize,
            _currentOrthoSize,
            ref _orthoVelocity,
            zoomSmoothTime
        );
        _camera.orthographicSize = _smoothedOrthoSize;

        var desiredPosition = player.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref _velocityRef,
            smoothTime
        );

        if (lookAtTarget) transform.LookAt(player);
    }

    void OnDisable() {
        _inputActions.Player.Disable();
    }

    void OnDestroy() {
        _inputActions.Dispose();
    }

    void HandleZoom() {
        var scrollY = _inputActions.Player.Zoom.ReadValue<float>();
        if (scrollY != 0) {
            _currentOrthoSize -= scrollY * zoomSensitivity;
            _currentOrthoSize = Mathf.Clamp(_currentOrthoSize, minOrthoSize, maxOrthoSize);
        }
    }
}
