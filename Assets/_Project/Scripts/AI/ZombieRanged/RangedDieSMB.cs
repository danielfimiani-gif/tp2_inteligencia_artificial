using System;
using UnityEngine;

class RangedDieSMB : StateMachineBehaviour {
    [SerializeField] private float despawnDelay = 2.5f;
    public static event Action<RangedBrain> OnRangedZombieDied;

    private RangedBrain _brain;
    private float _timer;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _brain ??= animator.GetComponent<RangedBrain>();
        _brain.Agent.isStopped = true;
        _brain.Agent.enabled = false;

        foreach (Collider c in _brain.GetComponentsInChildren<Collider>()) {
            c.enabled = false;
        }

        _timer = 0;
        OnRangedZombieDied?.Invoke(_brain);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _timer += Time.deltaTime;
        if (_timer >= stateInfo.length + despawnDelay) {
            Destroy(_brain.gameObject);
        }
    }
}