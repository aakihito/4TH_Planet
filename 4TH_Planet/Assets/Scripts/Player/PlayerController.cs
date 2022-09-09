using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _data;

    #region COMPONENTS

    private Rigidbody2D _rb;
    private Animator _anim;

    #endregion

    #region STATES

    private bool IsFacingRight;
    private bool IsJumping;
    private bool IsDashing;
    private bool IsAttacking;
    private bool IsSliding;
    private bool IsCrouching;

    private float LastOnGroundTime;
    private float LastOnWallTime;
    private float LastOnWallRightTime;
    private float LastOnWallLeftTime;    
    #endregion

    #region  INPUT PARAMETERS

    private float LastPressedJumpTime;
    private float LastPressedDashTime;
    private float LastPressedAttackTime;
    private float LastPressedCrouchTime;

    #endregion

    #region DASH

    private int _dashesLeft;
    private Vector2 _lastDashDir;
    private bool _dashAttacking;
    private bool _dashRefilling;

    #endregion

    [Header("Crouch")]

    [SerializeField] private BoxCollider2D _collider;

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

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();    
    }

    private void Start() 
    {
        InputHandler.instance.OnJumpPressed += args => OnJump(args);
        InputHandler.instance.OnJumpReleased += args => OnJumpUp(args);
        InputHandler.instance.OnCrouch += args => OnCrouch(args);
        InputHandler.instance.OnStandUp += args => OnStandUp(args);
        InputHandler.instance.OnDash += args => OnDash(args);
        InputHandler.instance.OnAttack += args => OnAttack(args);

        SetGravityScale(_data.gravityScale);
        IsFacingRight = true;
    }

    private void Update() 
    {
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;    
    
        //LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        LastPressedCrouchTime -= Time.deltaTime;
        LastPressedAttackTime -= Time.deltaTime;

      if (InputHandler.instance.MoveInput.x != 0)
			CheckDirectionToFace(InputHandler.instance.MoveInput.x > 0);

		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) 
				LastOnGroundTime = _data.coyoteTime; 

			//Right Wall Check
			if ((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight))
				LastOnWallRightTime = _data.coyoteTime;

			//Right Wall Check
			if ((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
				|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight))
				LastOnWallLeftTime = _data.coyoteTime;

			
			//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		if (!IsDashing)
		{
			if (_rb.velocity.y >= 0)
				SetGravityScale(_data.gravityScale);
			else if (InputHandler.instance.MoveInput.y < 0)
				SetGravityScale(_data.gravityScale * _data.quickFallGravityMult);
			else
				SetGravityScale(_data.gravityScale * _data.fallGravityMult);
		}

		if (IsJumping && _rb.velocity.y < 0)
		{
			IsJumping = false;
			//Debug.Break();
		}

		if (!IsDashing && !IsCrouching)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				Jump();
			}
		}

		if (CanDash() && LastPressedDashTime > 0)
		{

			Sleep(_data.dashSleepTime); 

			//If not direction pressed, dash forward
			if (InputHandler.instance.MoveInput != Vector2.zero)
				_lastDashDir = InputHandler.instance.MoveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



			IsDashing = true;
			IsJumping = false;
			IsCrouching= false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}

		if (CanSlide() && ((LastOnWallLeftTime > 0 && InputHandler.instance.MoveInput.x < 0) || (LastOnWallRightTime > 0 && InputHandler.instance.MoveInput.x > 0)))
			IsSliding = true;		
		else
			IsSliding = false;

			
		if(CanCrouch())
		{
			Crouch();
			IsDashing = false;
			IsJumping = false;
		}
		else if((Physics2D.OverlapBox(_roofCheckPoint.position, _roofCheckSize, 0, _groundLayer)))
		{
			IsCrouching = true;	
		}
		else
		{
			_collider.size = new Vector2(_collider.size.x, 1.4f);
			_collider.offset = new Vector2(_collider.offset.x, 0f);
		}
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
   
    public void OnJump(InputHandler.InputArgs args)
	{
		LastPressedJumpTime = _data.jumpBufferTime;
	}

	public void OnJumpUp(InputHandler.InputArgs args)
	{
		if (CanJumpCut())
			JumpCut();
	}

	public void OnDash(InputHandler.InputArgs args)
	{
		LastPressedDashTime = _data.dashBufferTime;
	}
	public void OnCrouch(InputHandler.InputArgs args)
	{
		IsCrouching = true;

	}
	public void OnAttack(InputHandler.InputArgs args)
	{

	}

	public void OnStandUp(InputHandler.InputArgs args)
	{
		IsCrouching = false;
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

	private void Drag(float amount)
	{
		Vector2 force = amount * _rb.velocity.normalized;
		force.x = Mathf.Min(Mathf.Abs(_rb.velocity.x), Mathf.Abs(force.x)); 
		force.y = Mathf.Min(Mathf.Abs(_rb.velocity.y), Mathf.Abs(force.y));
		force.x *= Mathf.Sign(_rb.velocity.x); 
		force.y *= Mathf.Sign(_rb.velocity.y);

		_rb.AddForce(-force, ForceMode2D.Impulse); 
	}

	private void Run(float lerpAmount)
	{
		
		float targetSpeed = InputHandler.instance.MoveInput.x * _data.runMaxSpeed;
		float speedDif = targetSpeed - _rb.velocity.x;

		float accelRate;

		#region CROUCH SPEED DIF

		if(IsCrouching)
		{
			targetSpeed = InputHandler.instance.MoveInput.x * _data.crouchMaxSpeed;

		}

		#endregion
		
		#region Acceleration Rate

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccel : _data.runDeccel;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? _data.runAccel * _data.accelInAir : _data.runDeccel * _data.deccelInAir;
		
		if (((_rb.velocity.x > targetSpeed && targetSpeed > 0.01f) || (_rb.velocity.x < targetSpeed && targetSpeed < -0.01f)) && _data.doKeepRunMomentum)
		{
			accelRate = 0; 
		}

		#endregion


		if(_data.doKeepRunMomentum && Mathf.Abs(_rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(_rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			accelRate = 0; 
		}

		#region Velocity Power
		float velPower;
		if (Mathf.Abs(targetSpeed) < 0.01f)
		{
			velPower = _data.stopPower;
		}
		else if (Mathf.Abs(_rb.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(_rb.velocity.x)))
		{
			velPower = _data.turnPower;
		}
		else
		{
			velPower = _data.accelPower;
		}
		#endregion

		
		float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
		movement = Mathf.Lerp(_rb.velocity.x, movement, lerpAmount);

		_rb.AddForce(movement * Vector2.right);


		if (InputHandler.instance.MoveInput.x != 0)
			CheckDirectionToFace(InputHandler.instance.MoveInput.x > 0);
	}

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

		if(IsCrouching)
		{
			_collider.size = new Vector2(_collider.size.x, 0.70f);
			_collider.offset = new Vector2(_collider.offset.x, -0.36f);
		}
	}

	private void Jump()
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		float force = _data.jumpForce;
		if (_rb.velocity.y < 0)
			force -= _rb.velocity.y;

		_rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}
	private void JumpCut()
	{
		_rb.AddForce(Vector2.down * _rb.velocity.y * (1 - _data.jumpCutMultiplier), ForceMode2D.Impulse);
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
		if (LastOnWallTime > 0 && !IsJumping && !IsDashing && LastOnGroundTime <= 1)
			return true;
		else
			return false;
	}

	private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanJumpCut()
    {
		return IsJumping && _rb.velocity.y > 0;
    }

	private bool CanCrouch()
	{
		return LastOnGroundTime > 0 && IsCrouching;
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

	/*	 private void OnCollisionEnter2D(Collision2D collision) 
    {
        if(collision.gameObject.CompareTag("Cristal"))
        {
			_dashesLeft = 1;
			LastOnGroundTime = 1;
		
			Destroy(collision.gameObject);

		}   
    }*/
}