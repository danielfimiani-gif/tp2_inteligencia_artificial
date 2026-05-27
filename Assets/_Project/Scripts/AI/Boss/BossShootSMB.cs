using UnityEngine;

class BossShootSMB : StateMachineBehaviour {
    [SerializeField] private float shootFrameNormalized = 0.4f;
    [SerializeField] private float flightTime = 0.6f;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 2f, 1f);

    private BossBrain _brain;
    private bool _shootFired;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponentInParent<BossBrain>();
        if (_brain == null) return;
        if (_brain.Agent.isOnNavMesh) {
            _brain.Agent.isStopped = true;
            _brain.Agent.velocity = Vector3.zero;
        }
        _shootFired = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain == null) _brain = animator.GetComponentInParent<BossBrain>();
        if (_brain == null || _brain.Target == null) return;

        Vector3 dir = _brain.Target.position - _brain.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01) {
            Quaternion desired = Quaternion.LookRotation(dir);
            _brain.transform.rotation = Quaternion.Slerp(
                _brain.transform.rotation, desired, 8f * Time.deltaTime);
        }

        float t = stateInfo.normalizedTime % 1f;
        if (!_shootFired && t >= shootFrameNormalized) {
            _shootFired = true;
            FireProjectile();
        }
    }

    private void FireProjectile() {
        if (_brain == null || _brain.Target == null) return;
        Vector3 spawn = _brain.transform.position
            + _brain.transform.right * spawnOffset.x
            + Vector3.up * spawnOffset.y
            + _brain.transform.forward * spawnOffset.z;
        EnemyProjectilePool.Instance.Launch(
            spawn,
            _brain.Target.position + Vector3.up * 1.0f,
            flightTime
        );
    }
}
