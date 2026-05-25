using UnityEngine;

class RangedShootSMB : StateMachineBehaviour {
    [SerializeField] private float shootFrameNormalized = 0.4f;
    [SerializeField] private float flightTime = 0.6f;

    private RangedBrain _brain;
    private bool _shootFired;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponent<RangedBrain>();
        if (_brain.Agent.isOnNavMesh) {
            _brain.Agent.isStopped = true;
            _brain.Agent.velocity = Vector3.zero;
        }
        _shootFired = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain.Target != null) {
            var dir = _brain.Target.position - _brain.transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01) {
                var desired = Quaternion.LookRotation(dir);
                _brain.transform.rotation = Quaternion.Slerp(
                    _brain.transform.rotation,
                    desired,
                    8f * Time.deltaTime
                );
            }
        }

        float t = stateInfo.normalizedTime % 1f;
        if (!_shootFired && t >= shootFrameNormalized) {
            _shootFired = true;
            FireProjectile();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain.Agent.isOnNavMesh)
            _brain.Agent.isStopped = false;
    }

    void FireProjectile() {
        if (_brain.ProjectileSpawnPoint == null) return;
        EnemyProjectilePool.Instance.Launch(
            _brain.ProjectileSpawnPoint.position,
            _brain.Target.position + Vector3.up * 1.0f,
            flightTime
        );
    }
}