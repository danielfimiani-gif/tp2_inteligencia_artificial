using UnityEngine;

class BossDieSMB : StateMachineBehaviour {
    [SerializeField] private float despawnDelay = 4f;

    private BossBrain _brain;
    private float _timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponentInParent<BossBrain>();
        if (_brain == null) return;
        if (_brain.Agent != null && _brain.Agent.isOnNavMesh) {
            _brain.Agent.isStopped = true;
        }
        if (_brain.Agent != null) _brain.Agent.enabled = false;

        foreach (Collider c in _brain.GetComponentsInChildren<Collider>()) {
            c.enabled = false;
        }

        _timer = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain == null) return;
        _timer += Time.deltaTime;
        if (_timer >= stateInfo.length + despawnDelay) {
            Destroy(_brain.gameObject);
        }
    }
}
