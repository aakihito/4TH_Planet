using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControllerTest : MonoBehaviour
{
     #region  OTHER SCRIPTS
    public static PlayerControllerTest instance;
    private GameControls _controls;
    [SerializeField] private PlayerData _data;
	private ResetPlayerPrefabInDeath _resetedPrefab;
	private PlayerAnimatorController _PlayerAnim;
	private HudController _hudManager;
	#endregion

	#region COMPONENTS
    public Rigidbody2D _rb {get; private set; } 
	public SpriteRenderer spriteR {get; private set; }

    #endregion

    #region STATES

    public bool IsFacingRight {get; private set; }
    public bool IsJumping {get; private set; }
    public bool IsDashing {get; private set; }
    public bool IsAttacking {get; private set; }
    public bool IsSliding {get; private set; }
    public bool IsCrouching;
	public  bool IsGrounded {get; private set; }
	public bool IsDamaged {get; private set; }
	public bool IsDead {get; private set; }

    private float LastOnGroundTime;
    private float LastOnWallTime;
    private float LastOnWallRightTime;
    private float LastOnWallLeftTime;    
    #endregion

    #region  INPUT PARAMETERS

    private float LastPressedJumpTime;
    private float LastPressedDashTime;
    private float LastPressedCrouchTime;
	private float PressedAttackTime;

    #endregion

    #region DASH

    private int _dashesLeft;
    private Vector2 _lastDashDir;
    private bool _dashAttacking;
    private bool _dashRefilling;

    #endregion

	#region RUN

    public Vector2 MoveInput { get; private set; }
	public float targetSpeed {get; private set;}
	public float force {get; private set;}

	#endregion

	#region  JUMP
	private bool _isJumpCut;
	private bool _isJumpFalling;

	#endregion

    [Header("Crouch")]

    [SerializeField] private BoxCollider2D _collider;

	#region ATTACK

	[Header("Attack")]

	[SerializeField] private BoxCollider2D _attackCol;

	#endregion
    

    #region CHECKS

    [Header("Checks")]

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    
    [SerializeField] private Transform _roofCheckPoint;
    [SerializeField] private Vector2 _roofCheckSize;

    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize;

    #endregion

    #region LAYERS & TAGS

    [Header("Layers & Tags")]

    [SerializeField] private LayerMask _groundLayer;

    #endregion

	 #region SPAWNS and Fall 
	

	public Vector3 _respawnPoint {get; private set;}

	[Header("Spawns")]
	[SerializeField] private GameObject _fallDetect;

	#endregion

    private void Awake()
	{
        #region GET COMPONENTS

        _rb = GetComponent<Rigidbody2D>();   
		spriteR = GetComponent<SpriteRenderer>();

        #endregion

		#region Singleton
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		#endregion

		_controls = new GameControls();

		#region Assign Inputs

		_controls.Player.Walk.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
		_controls.Player.Walk.canceled += ctx => MoveInput = Vector2.zero;

		_controls.Player.Jump.performed += ctx => OnJumpPressed();
		_controls.Player.JumpUp.performed += ctx => OnJumpReleased();
		_controls.Player.Dash.performed += ctx => OnDash();
		_controls.Player.Crouch.started += ctx =>OnCrouch();
		_controls.Player.Crouch.performed += ctx => OnCrawling();
		_controls.Player.Crouch.canceled += ctx => OnStandUp();
		_controls.Player.Attack.performed += ctx => OnAttack();

		#endregion
	}

    #region INPUT DISABLE/ENABLE

    private void OnEnable()
	{
		_controls.Enable();
	}

	private void OnDisable()
	{
		_controls.Disable();
	}

    #endregion

     private void Start() 
    {
         _resetedPrefab = ResetPlayerPrefabInDeath.resetPlayerPrefab;

        SetGravityScale(_data.gravityScale);
        IsFacingRight = true;
		//_PlayerAnim = PlayerAnimatorController.PlayerAnim;
		_respawnPoint = transform.position;
    }

    private void Update() 
    {
		#region FALL CHECK
		_fallDetect.transform.position = new Vector2(transform.position.x, _fallDetect.transform.position.y);
		#endregion

        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;    
    
        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        LastPressedCrouchTime -= Time.deltaTime;
		PressedAttackTime -= Time.deltaTime;

      if (MoveInput.x != 0)
			CheckDirectionToFace(MoveInput.x > 0);		

		if (!IsDashing && !IsJumping && !IsCrouching)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping)
			{
				IsGrounded = true;
				LastOnGroundTime = _data.coyoteTime; 
            }		

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)))
				LastOnWallRightTime = _data.coyoteTime;

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)))
				LastOnWallLeftTime = _data.coyoteTime;

			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}

		#region JUMP CHECKS

		if (IsJumping && _rb.velocity.y < 0)
		{
			IsJumping = false;
			_isJumpFalling = true;
		}

		if (LastOnGroundTime > 0 && !IsJumping)
        {
			_isJumpCut = false;

			if(!IsJumping)
				_isJumpFalling = false;
		}

		if (!IsDashing && !IsCrouching)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}
		}

		#endregion

		if (CanDash() && LastPressedDashTime > 0)
		{

			Sleep(_data.dashSleepTime); 
			if (MoveInput != Vector2.zero)
				_lastDashDir = MoveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



			IsDashing = true;
			IsJumping = false;
			IsCrouching = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		/*if(CanAttack() && PressedAttackTime > 1)
		{
			Attack();
		}*/

		if (CanSlide() && ((LastOnWallLeftTime > 0 && MoveInput.x < 0) || (LastOnWallRightTime > 0 && MoveInput.x > 0)))
			IsSliding = true;		
		else
			IsSliding = false;

			
		if(CanCrouch() && LastOnGroundTime > 0)
		{
			Crouch();
		}
		
		if((Physics2D.OverlapBox(_roofCheckPoint.position, _roofCheckSize, 0, _groundLayer)))
		{
			if(CanCrouch() && LastOnGroundTime > 0)
			{
				Crouch();
			}
		}
		else if(!IsCrouching)
		{
			_collider.size = new Vector2(_collider.size.x,0.74f);
			_collider.offset = new Vector2(_collider.offset.x, 0f);
			
			PlayerLifeController.boxCol.size = new Vector2(PlayerLifeController.boxCol.size.x, 0.74f);
			PlayerLifeController.boxCol.offset = new Vector2(PlayerLifeController.boxCol.offset.x, 0f);
		}

		#region GRAVITY
		if (!_dashAttacking)
		{
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (_rb.velocity.y < 0 && MoveInput.y < 0)
			{
				SetGravityScale(_data.gravityScale * _data.fastFallGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				SetGravityScale(_data.gravityScale * _data.jumpCutGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
			}
			else if ((IsJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold)
			{
				SetGravityScale(_data.gravityScale * _data.jumpHangGravityMult);
			}
			else if (_rb.velocity.y < 0)
			{
				SetGravityScale(_data.gravityScale * _data.fallGravityMult);
				_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_data.maxFallSpeed));
			}
			else
			{
				SetGravityScale(_data.gravityScale);
			}
		}
		else
		{
			SetGravityScale(0);
		}
		#endregion
	}

	private void FixedUpdate()
	{
        if (!IsDashing)
		{
			Run(1);
			
		}
		else if (_dashAttacking)
		{
			Run(_data.dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
    }

    #region INPUT CALLBACKS
   
    public void OnJumpPressed()
	{
        Debug.Log("Tá funcionando o Pulo");
		LastPressedJumpTime = _data.jumpBufferTime;
	}

	public void OnJumpReleased()
	{
        Debug.Log("Tá funcionando o pulo soltando");
		if (CanJumpCut())
			_isJumpCut = true;
	}

	public void OnDash()
	{
        Debug.Log("Tá funcionando o dash");
		LastPressedDashTime = _data.dashBufferTime;
	}
	public void OnCrouch()
	{
        Debug.Log("Tá funcionando o crouch");
		IsCrouching = true;
	}
	public void OnCrawling()
	{
        Debug.Log("Tá funcionando o engatinhar");
		IsCrouching = true;
	}
	public void OnStandUp()
	{
        Debug.Log("Tá funcionando o levantar");
		IsCrouching = false;
	}
	public void OnAttack()
	{
		//PressedAttackTime = _data.attackCooldown;
	}


    #endregion

    #region MOVEMENT METHODS

    public void SetGravityScale(float scale)
	{
		_rb.gravityScale = scale;
	}

	private void Sleep(float duration)
    {
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1;
	}

	private void Run(float lerpAmount)
	{
		targetSpeed = MoveInput.x * _data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(_rb.velocity.x, targetSpeed, lerpAmount);

		if(IsCrouching)
		{
			targetSpeed = MoveInput.x * _data.crouchMaxSpeed;

		}
		else if(IsAttacking)
		{
			targetSpeed = MoveInput.x * 0;
		}

		#region Calculate AccelRate
		float accelRate;

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount : _data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccelAmount * _data.accelInAir : _data.runDeccelAmount * _data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration

		if ((IsJumping || _isJumpFalling) && Mathf.Abs(_rb.velocity.y) < _data.jumpHangTimeThreshold)
		{
			accelRate *= _data.jumpHangAccelerationMult;
			targetSpeed *= _data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum

		if(_data.doConserveMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			accelRate = 0; 
		}
		#endregion

		float speedDif = targetSpeed - _rb.velocity.x;

		float movement = speedDif * accelRate;

		_rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

		if (MoveInput.x != 0)
			CheckDirectionToFace(MoveInput.x > 0);
	}

	/*private void Attack()
	{
		PressedAttackTime = 0;
		//PlayerLifeController.boxCol.enabled = false;
		_attackCol.enabled = true;
		AttackCoroutine();
		IsAttacking = false;
	}
	private void AttackCoroutine()
	{
		StartCoroutine(AttackDuration());
	}
	private IEnumerator AttackDuration()
	{
		yield return new WaitForSeconds(0.2f);
		_attackCol.enabled = false;
	}*/
	
	private void Turn()
	{
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}

	private void Crouch()
	{
		LastOnGroundTime = 0;
		IsGrounded = true;
		_collider.size = new Vector2(_collider.size.x,  0.09f);
		_collider.offset = new Vector2(_collider.offset.x, -0.32f);
		PlayerLifeController.boxCol.size = new Vector2(PlayerLifeController.boxCol.size.x, 0.09f);
		PlayerLifeController.boxCol.offset = new Vector2(PlayerLifeController.boxCol.offset.x,-0.32f);
	}

	private void Jump()
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		IsGrounded = false;

		#region Perform Jump
		
		force = _data.jumpForce;
		if (_rb.velocity.y < 0)
			force -= _rb.velocity.y;

		_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}
	private void Slide()
	{
		float speedDif = _data.slideSpeed - _rb.velocity.y;	
		float movement = speedDif * _data.slideAccel;

		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		_rb.AddForce(movement * Vector2.up);
	}

	private IEnumerator StartDash(Vector2 dir)
	{

		LastOnGroundTime = 0;
		IsGrounded = false;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_dashAttacking = true;

		SetGravityScale(0);

		while (Time.time - startTime <= _data.dashAttackTime)
		{
			_rb.velocity = dir.normalized * _data.dashSpeed;
			yield return null;
		}

		startTime = Time.time;

		_dashAttacking = false;

		SetGravityScale(_data.gravityScale);
		_rb.velocity = _data.dashEndTime * dir.normalized;

		while (Time.time - startTime <= _data.dashEndTime)
		{
			yield return null;
		}

		IsDashing = false;
	}

private IEnumerator RefillDash(int amount)
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(_data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(_data.dashAmount, _dashesLeft + 1);
	}
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	private bool CanSlide()
	{
		if (LastOnWallTime > 0 && !IsJumping && !IsDashing && !IsCrouching && LastOnGroundTime <= 1)
			return true;
		else
			return false;
	}

	private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }
	private bool CanAttack()
	{
		return LastOnGroundTime > 0 && !IsAttacking;
	}
	private bool CanJumpCut()
    {
		return IsJumping && _rb.velocity.y > 0;
    }
	private bool CanCrouch()
	{
		return IsCrouching;
	}
	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < _data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	} 
	#endregion

	private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.gameObject.CompareTag("Cristal"))
        {
			_dashesLeft = 1;
			LastOnGroundTime = 0;
		
			Destroy(collision.gameObject);
		}   
    }

	public IEnumerator DamagePlayer()
	{
		 IsDamaged = true;
		 spriteR.color = new Color(1f, 0, 0, 1f);
		 yield return new WaitForSeconds(0.2f);
		 spriteR.color = new Color(1f, 1f, 1f, 1f);
		 IsDamaged = false; 

		 for(int i = 0; i < 4; i++)
		 {
			spriteR.enabled = false;
			yield return new WaitForSeconds(0.15f);
			spriteR.enabled = true;
			yield return new WaitForSeconds(0.15f);
		 }

		 PlayerLifeController.boxCol.enabled = true;
	}

	public void PlayerDeath()
	{
		IsDead = true;
		PlayerLifeController.boxCol.enabled = false;
		Destroy(gameObject);
		Reseted();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		_resetedPrefab.Respawn();
		_hudManager.life = 6;
		PlayerLifeController.boxCol.enabled = true;


	}
	private void Reseted()
	{
		StartCoroutine(ResetScene());
	}
	private IEnumerator ResetScene()
	{
		yield return new WaitForSeconds(2f);
		
	}
}

