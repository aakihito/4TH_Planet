using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterAnimatorKeys
{
    public const string IsCrouching = "Iscrouching";
    public const string HorizontalSpeed = "HorizontalSpeed";
    public const string IsJumping = "IsJumping";
    public const string IsGrounded = "IsGrounded";
    public const string IsDashing = "IsDashing";
}
public class PlayerAnimatorController : MonoBehaviour
{
    public static PlayerAnimatorController PlayerAnim {get; private set;}

   Animator _animator;
   PlayerControllerTest _controller;
   

    private void Awake() 
   {
        _animator = GetComponent<Animator>();
        _controller = PlayerControllerTest.instance;
        

        if (PlayerAnim == null)
        {
            PlayerAnim = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
   }
   
   private void Update() 
   {
        if(_controller.MoveInput.x != 0)
        {

            _animator.SetFloat(CharacterAnimatorKeys.HorizontalSpeed, _controller.MoveInput.x / _controller.targetSpeed);
        }

        else
        {
            _animator.SetFloat(CharacterAnimatorKeys.HorizontalSpeed, 0f);
        }

        _animator.SetBool(CharacterAnimatorKeys.IsJumping , _controller.IsJumping);

        if(!_controller.IsDashing && !_controller.IsJumping && !_controller.IsCrouching && !_controller.IsSliding)
        {
            _animator.SetBool(CharacterAnimatorKeys.IsGrounded, _controller.IsGrounded);
        }
        if(_controller.IsCrouching)
        {
            _animator.SetBool(CharacterAnimatorKeys.IsCrouching, true);
        }
        else
        {
            _animator.SetBool(CharacterAnimatorKeys.IsCrouching, false);
        }
        _animator.SetBool(CharacterAnimatorKeys.IsDashing, _controller.IsDashing);

    }
} 
