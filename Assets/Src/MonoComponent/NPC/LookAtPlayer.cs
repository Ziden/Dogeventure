using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Animator _anim;
    private static readonly int Idle = Animator.StringToHash("idle");
    private Player _player;
    private Vector3 _lookOffset = new (0, 2.5f, 0);
    
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetBool(Idle, true);
        Map.Current.OnPlayerInitialized += p => _player = p;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_player == null) return;
        var pos = _player.Collider.bounds.center;
        if (Vector3.Distance(transform.position, pos) > 20f) return;
        _anim.SetLookAtWeight(1f, 0.1f, 1f);
        _anim.SetLookAtPosition(new Vector3(pos.x, transform.position.y, pos.z) + _lookOffset);
    }
}
