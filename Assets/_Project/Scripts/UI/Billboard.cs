using UnityEngine;

class Billboard : MonoBehaviour {
    private Camera _camera;

    void Start() {
        _camera = Camera.main;
    }

    void LateUpdate() {
        transform.rotation = _camera.transform.rotation;
    }
}