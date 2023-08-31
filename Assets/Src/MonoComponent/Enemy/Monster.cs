using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using GameAddressables;
using Src;
using Src.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Sequence = DG.Tweening.Sequence;

public class Monster : MonoBehaviour
{
	public event Action OnSeePlayer;
	public event Action OnStopSeeingPlayer;
	public event Action OnTouchGround;
	public event Action OnLeaveGround;
	
	public float Sight = 9f;
	private bool _seeingPlayer = false;
	private bool _grounded = false;
	private Rigidbody _body;
	private LivingEntity _entity;
	private AttackerEntity _attacker;
	private Vector3 _spawn;
	private float _playerDistance;
	private GameObject _floor;
	private List<DamageDealer> _damageDealers = new();

	public AttackerEntity AttackerEntity => _attacker;
	
	private static Dictionary<Vector3, DateTime> _killed = new();

	public IReadOnlyCollection<DamageDealer> DamageDealers => _damageDealers;
	
	void Awake()
	{
		gameObject.GetOrAddComponent<LivingEntity>();
		gameObject.GetOrAddComponent<AttackerEntity>();
		gameObject.GetOrAddComponent<AttackableEntity>();
	}
	
	void Start()
    {
	    _body = GetComponent<Rigidbody>();
	    _entity = GetComponent<LivingEntity>();
	    _attacker = GetComponent<AttackerEntity>();
	    _entity.OnSpawn += OnSpawned;
	    _entity.OnPlaySequence += OnPlaySequence;
	    _entity.OnDie += OnDie;
	    _damageDealers = gameObject.GetComponentsInChildren<DamageDealer>().ToList();
    }

	private void OnDie()
	{
		_killed[_spawn] = DateTime.UtcNow;
	}

	private bool HasRecentlyKilled()
	{
		return _killed.TryGetValue(_spawn, out var d) 
		       && DateTime.UtcNow < d + TimeSpan.FromMinutes(20);
	}

	private void OnPlaySequence(Sequence s)
	{
		if (_body == null) return;
		s.OnStart(() =>
		{
			_body.velocity = Vector3.zero;
			_body.angularVelocity = Vector3.zero;
		});
	}

	private void OnSpawned(LivingEntity e)
	{
		_spawn = e.transform.position;
		if (HasRecentlyKilled())
		{
			Destroy(gameObject);
			return;
		}
		_entity.AttackableEntity.OnAttacked += OnAttacked;
	}

	void OnDestroy()
	{
		Main.Services.Map.RemoveAggro(this);
	}

	private void OnAttacked(DamageDealer dealer)
	{
		Main.Services.Audio.PlaySoundEffect(AssetSoundEffect.Interface5, 1, 0.2f);
		var blinker = gameObject.GetOrAddComponent<Blinker>();
		blinker.StartBlink(0.4f);
		Main.Services.Vfx.Play(VfxPrefab.CfxR2GroundHit, _entity.Collider.bounds.center, fx =>
		{
			fx.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		});
	}

	public Rigidbody Body => _body;
	public bool SeeingPlayer => _seeingPlayer;
	public bool IsOnGround => _grounded || (_body.isKinematic);

	private void OnCollisionEnter(Collision collision)
    {
	    if (collision.gameObject.CompareTag("Ground"))
	    {
		    _floor = collision.gameObject;
		    _grounded = true;
		    OnTouchGround?.Invoke();
	    }
    }

	private void OnCollisionExit(Collision collision)
    {
	    if (collision.gameObject.CompareTag("Ground") && collision.gameObject == _floor)
	    {
		    _floor = null;
		    _grounded = false;
		    OnLeaveGround?.Invoke();
	    }
    }
	
	public void DisableSomePhysics()
	{
		_body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
	}

	public void EnableSomePhysics()
	{
		_body.constraints = RigidbodyConstraints.None;
	}
    
    void Update()
    {
	    if (!Main.Loaded || Main.Services.Map.GameFrozen) return;
	    
	    var p = Player.Get();
	    if (p == null) return;

	    _playerDistance = Vector3.Distance(p.transform.position, transform.position);
	    if (_playerDistance > Sight)
	    {

		    if (_seeingPlayer)
		    {
			    _seeingPlayer = false;
			    OnStopSeeingPlayer?.Invoke();
			    Main.Services.Map.RemoveAggro(this);
		    }
	    }
	    else if(!_seeingPlayer)
	    {
		    _seeingPlayer = true;
		    OnSeePlayer?.Invoke();
		    Main.Services.Map.AddAggro(this);
	    }
    }
}
