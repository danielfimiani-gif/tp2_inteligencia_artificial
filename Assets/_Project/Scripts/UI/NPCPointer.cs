using UnityEngine;

public class NPCPointer : MonoBehaviour {
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float screenMargin = 80f;
    [SerializeField] private bool hideWhenOnScreen = true;

    private Camera _cam;

    void Start() {
        _cam = Camera.main;
        arrow.gameObject.SetActive(false);
    }

    void LateUpdate() {
        var npc = WaveManager.Instance?.CurrentNpc;
        if (npc == null) {
            arrow.gameObject.SetActive(false);
            return;
        }

        Vector3 targetPos = npc.transform.position;

        if (hideWhenOnScreen) {
            Vector3 viewport = _cam.WorldToViewportPoint(targetPos);
            bool onScreen = viewport.z > 0
                            && viewport.x > 0.05f && viewport.x < 0.95f
                            && viewport.y > 0.05f && viewport.y < 0.95f;
            if (onScreen) {
                arrow.gameObject.SetActive(false);
                return;
            }
        }
        arrow.gameObject.SetActive(true);

        Vector3 camRight = _cam.transform.right;
        Vector3 camForward = _cam.transform.forward;
        camRight.y = 0; camRight.Normalize();
        camForward.y = 0; camForward.Normalize();

        Vector3 worldDir = targetPos - _cam.transform.position;
        worldDir.y = 0;

        float x = Vector3.Dot(worldDir, camRight);
        float y = Vector3.Dot(worldDir, camForward);

        Vector2 screenDir = new Vector2(x, y);
        if (screenDir.sqrMagnitude < 0.0001f) return;
        screenDir.Normalize();

        float halfW = Screen.width / 2f - screenMargin;
        float halfH = Screen.height / 2f - screenMargin;
        float scaleX = halfW / Mathf.Max(Mathf.Abs(screenDir.x), 0.001f);
        float scaleY = halfH / Mathf.Max(Mathf.Abs(screenDir.y), 0.001f);
        float scale = Mathf.Min(scaleX, scaleY);

        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        arrow.position = center + screenDir * scale;

        float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg - 90f;
        arrow.localEulerAngles = new Vector3(0, 0, angle);
    }
}
