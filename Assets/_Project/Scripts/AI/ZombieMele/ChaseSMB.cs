using UnityEngine;

class ChaseSMB : StateMachineBehaviour {
    private ZombieBrain _brain;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponent<ZombieBrain>();
        _brain.Agent.isStopped = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (_brain.Target == null) return;
        if (!_brain.Agent.isOnNavMesh) return;
        _brain.Agent.SetDestination(_brain.Target.position);
    }
}