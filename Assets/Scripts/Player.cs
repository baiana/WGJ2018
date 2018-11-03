using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

	[SerializeField] private float _moveLockDuration = .5f;
	[SerializeField] private float _groundCheckRadius = .25f;
	[SerializeField] private float _moveSpeed = 6f;
	[SerializeField] private float _jumpForce = 12f;
	[SerializeField] private float _awayTimeout = 5f;
	[SerializeField] private float _jumpBuffer = .125f;
	[SerializeField] private float _groundCheckHeight = .25f;
	[SerializeField] private float _minimumJump = 12f;
	[SerializeField] private float _jumpCooldown = .125f;
	[SerializeField] [Range(0f, 1f)] private float _jumpSustain = .5f;
	//[SerializeField] private LayerMask _whatIsGround;
	//[SerializeField] private GameObject _deathParticles;
	//[SerializeField] private ParticleSystem _dustParticles;
	//[SerializeField] private AudioClip _jumpSFX;
	//[SerializeField] private AudioClip _deathSFX;

	private bool _inputJump;
	private bool _inputReleaseJump;
	private bool _canMove;
	private bool _isGrounded;
	private bool _isWinner;
	private bool _hasReleasedJump;
	private float _inputMovement;
	private float _nextAwayTime;
	private float _delayJumpTime;
	private float _nextJumpTime;
	//private Animator _animator;
	private Rigidbody2D _rigidbody;
	private SpriteRenderer _spriteRenderer;
	private CircleCollider2D _collider;
	//private ParticleSystem.EmissionModule _dustEmission;
	private bool _isDead;
	private Vector3 _startPosition;
	private float _moveLockTime;
	private float _jumpSFXTime;


	 void Awake() {
		_rigidbody = GetComponent<Rigidbody2D>();
		//_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_collider = GetComponent<CircleCollider2D>();
		//_dustEmission = _dustParticles.emission;
		SetMovement(true);
	}

	public void SetMovement(bool value)
	{
		_canMove = value; 
		_collider.enabled = value;
		_rigidbody.bodyType = value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
	
	}


	void Start () {
		_startPosition = transform.position;
		_moveLockTime = _moveLockDuration;
	}
	
	void Update () {
			GetInput();
		if (Time.time < _moveLockTime) return;
		ProcessInput();
	}

	private void GetInput()
	{
		_inputMovement = Input.GetAxisRaw("Horizontal");

		if (_inputMovement > 0f)
			_inputMovement = 1f;
		else if (_inputMovement < 0f)
			_inputMovement = -1f;

		_inputJump = Input.GetButtonDown("Jump");
		_inputReleaseJump = Input.GetButtonUp("Jump");
		ProcessPhysicsInput();
	}

	private void ProcessInput()
	{
		if (!_canMove) return;

		if (_inputJump) Jump();
		if (_inputReleaseJump) ReleaseJump();
		if (_hasReleasedJump) CheckRelease();
	}

private void ReleaseJump()
	{
		_hasReleasedJump = true;
	}

private void CheckRelease()
	{
		var currentVelocity = _rigidbody.velocity;

		if (currentVelocity.y > 0 && currentVelocity.y < _minimumJump)
		{
			currentVelocity.y *= _jumpSustain;
			_rigidbody.velocity = currentVelocity;
			_hasReleasedJump = false;
		}
		else if (currentVelocity.y <= 0)
		{
			_hasReleasedJump = false;
		}
	}

 private void OnCollisionExit2D(Collision2D other) {
	if(other.gameObject.CompareTag("Plataform")){
			_isGrounded = false;
			_nextAwayTime = Time.time + _awayTimeout;
		}
}
private void OnCollisionStay2D(Collision2D other) {
	if(other.gameObject.CompareTag("Plataform")){
			_isGrounded = true;
		}
}
	private void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.CompareTag("Plataform")){
			_isGrounded = true;
		}
	}
	private void CheckForGround()
	{
		var checkPosition = transform.position - Vector3.up * _groundCheckHeight;
		if (_isGrounded)
		{
			_delayJumpTime = Time.time + _jumpBuffer;
		}
		else
		{
			if (Time.time < _delayJumpTime) _isGrounded = true;
		}
	}

	
	private void ProcessPhysicsInput()
	{
		if (!_canMove) return;
		var currentVelocity = _rigidbody.velocity;
		currentVelocity.x = _inputMovement * _moveSpeed;
		_rigidbody.velocity = currentVelocity;
	}

		private void FixedUpdate()
	{
		CheckForGround();
		//AwayCheck();

		if (Time.time < _moveLockTime) return;

		ProcessPhysicsInput();
	}
		private void Jump()
	{
		if (!_canMove) return;
		if (!_isGrounded) return;
		if (Time.time < _nextJumpTime) return;

		var currentVelocity = _rigidbody.velocity;
		currentVelocity.y = _jumpForce;
		_rigidbody.velocity = currentVelocity;
		
		_isGrounded = false;
		_nextJumpTime = _jumpCooldown + Time.time;
		_delayJumpTime = 0f;
	}
}
