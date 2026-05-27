using UnityEngine;

class BossRetreatSMB : StateMachineBehaviour {
    [SerializeField] private float retreatDistance = 10f;

    private BossBrain _brain;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponentInParent<BossBrain>();
        if (_brain == null) return;
        if (_brain.Agent.isOnNavMesh) _brain.Agent.isStopped = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain == null) _brain = animator.GetComponentInParent<BossBrain>();
        if (_brain == null || _brain.Target == null) return;
        if (!_brain.Agent.isOnNavMesh) return;

        Vector3 awayDir = _brain.transform.position - _brain.Target.position;
        awayDir.y = 0;
        if (awayDir.sqrMagnitude < 0.01f) return;
        awayDir.Normalize();

        Vector3 retreatPos = _brain.transform.position + awayDir * retreatDistance;
        _brain.Agent.SetDestination(retreatPos);
    }
}
