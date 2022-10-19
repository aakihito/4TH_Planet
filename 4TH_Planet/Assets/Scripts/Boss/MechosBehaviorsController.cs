using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechosBehaviorsController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _groundCheckSize;
    public bool _isGrounded;
    [SerializeField] private float _chasingSpeed;


     #region ATTACK

    private Transform _player;

    [Header("JUMP ATTACK")]
    
    [SerializeField] private float _jumpForce;

    private bool _isJumping;
    public bool _jumpAttack;
    [SerializeField] private float _lineOfSight;
    [SerializeField] private float _shootingRange;
    [SerializeField] private float _fallingSpeed;

    #endregion
    
     private void Awake() 
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;   
    }

     private void Update() 
    {
        _isGrounded =  (Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayer));

    }

   
    private void FixedUpdate() 
    {
        #region Follow Player

        float distanceFromPlayer = Vector2.Distance(_player.position, transform.position);

        if(distanceFromPlayer < _lineOfSight && distanceFromPlayer > _shootingRange && !_jumpAttack)
        {
            FollowPlayer();
            _rb.gravityScale = 1f;

        }

        if(distanceFromPlayer > _lineOfSight && distanceFromPlayer > _shootingRange)
        {
            StartCoroutine(JumpAttack());
        }

        if(!_isGrounded && _jumpAttack)
        {
            FallingInPlayer();  
            StartCoroutine(ResetGravity());
            _rb.gravityScale = 0.2f;


        }

        #endregion    
    }

   /* private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Attack")
        {
            EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        _life = 0; 
        Destroy(transform.gameObject.GetComponent<BoxCollider2D>());
       
       Destroy(transform.gameObject.GetComponent<Rigidbody2D>());
    }
    */

    private void FollowPlayer()
    {
        Vector2 startPos = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 endPos = new Vector2(_player.transform.position.x, this.transform.position.y);
        transform.position = Vector2.MoveTowards(startPos, endPos, _chasingSpeed * Time.deltaTime);
    }

    private void FallingInPlayer()
    {
        Vector2 startPos = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 endPos = new Vector2(_player.transform.position.x, _player.transform.position.y);
        transform.position = Vector2.MoveTowards(startPos, endPos, _fallingSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, _lineOfSight);
        Gizmos.DrawWireSphere(transform.position, _shootingRange);  
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

    private IEnumerator ResetGravity()
    {
        yield return new WaitForSeconds(1f);
        _jumpAttack = false;
    }

}
