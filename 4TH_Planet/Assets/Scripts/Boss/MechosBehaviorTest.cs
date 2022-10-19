using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechosBehaviorTest : MonoBehaviour
{

    #region  COMPONENTS and PLAYER

    [SerializeField] private SpriteRenderer _spriteRend;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private BoxCollider2D _boxCol;
    [SerializeField] private Transform _player;

    #endregion

    #region TAGS & LAYERS 

    [Header("TAGS & LAYER")]

    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _wallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize;
    [SerializeField] private LayerMask _groundLayer;

    #endregion

    #region STATES

    public bool IsIdle;
    private bool IsGrounded;
    private bool IsTouchingWall;
    private bool IsJumping;
    private bool IsAttacking;
    private bool IsDying;
    private bool IsJumpAtacking;
    private bool IsFacingLeft;

    #endregion


    #region  WALK
    [Header("WALK  AND IDLE")]

    [SerializeField] private Vector2 _walkSpeed;

    #endregion


    #region JUMP
    [Header("JUMP")]

    
    #endregion

    #region JUMP ATTACK
    [Header("JUMP ATTACK")]

    #endregion

    #region  ATTACK
    [Header("ATTACK")]
    [SerializeField] private float _attackPlayerSpeed;

    #endregion

    #region REPEATER

    #endregion


    private void Update() 
    {
        IsTouchingWall = Physics2D.OverlapBox(_wallCheckPoint.position, _wallCheckSize, 0, _groundLayer);
        IsGrounded = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);  

        #region GRAVITY

            if(IsGrounded)
            {

            }
        #endregion



        

        #region  FLIP 

        #endregion
    }


    #region METHODS

    void IdleState()
    {

    }

   private IEnumerator JumpAttack()
    {
       _isJumping = true; 

        if(_isJumping && _isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            _rb.gravityScale = 0;
            yield return new WaitForSeconds(2f);
            _jumpAttack = true; 
        }    
    }
    private void FallingInPlayer()
    {
        Vector2 startPos = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 endPos = new Vector2(_player.transform.position.x, _player.transform.position.y);
        transform.position = Vector2.MoveTowards(startPos, endPos, _fallingSpeed * Time.deltaTime);
        _rb.gravityScale = 0.2f;
    }

    void Jump()
    {
         if(_isJumping && _isGrounded)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
        }    
    }







  /*  void Flip()
    {
        IsFacingLeft = !IsFacingLeft;
        _walkSpeed.x *= -1;
        transform.Rotate(0,180,0);    
    }
      OU

    void Flip()
    {
        Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingLeft = !IsFacingLeft;
    }
    */
 
    #endregion


    #region DiSABLE ATTACK VARIABLE

    void DisableAttack()
    {
        IsJumpAtacking = false;
        IsAttacking = false;
    }
    #endregion


}
