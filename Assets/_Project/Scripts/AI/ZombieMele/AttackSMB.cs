using UnityEngine;
using UnityEngine.Rendering;

class AttackSMB : StateMachineBehaviour {
    [SerializeField] private float damageFrameNormalized = 0.5f;

    private ZombieBrain _brain;
    private bool _damageDealt;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponent<ZombieBrain>();
        _brain.Agent.isStopped = true;
        _brain.Agent.velocity = Vector3.zero;
        _damageDealt = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain.Target != null) {
            Vector3 dir = _brain.Target.position - _brain.transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01) {
                Quaternion desired = Quaternion.LookRotation(dir);
                _brain.transform.rotation = Quaternion.Slerp(
                    _brain.transform.rotation,
                    desired,
                    8f * Time.deltaTime
                );
            }
        }

        float t = stateInfo.normalizedTime % 1f;
        if (!_damageDealt && t >= damageFrameNormalized) {
            _damageDealt = true;
            TryDealDamage();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain.Agent.isStopped = false;
    }

    private void TryDealDamage() {
        if (_brain.Target == null) return;
        float dist = Vector3.Distance(_brain.transform.position, _brain.Target.position);
        if (dist <= 2.0f) {
            var dmg = _brain.Target.GetComponent<IDamageable>();
            dmg?.ReceiveDamage(_brain.AttackDamage);
        }
    }
}