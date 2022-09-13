using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterAnimatorKeys
{
    public const string IsCrouching = "Iscrouching";
    public const string HorizontalSpeed = "HorizontalSpeed";
    public const string VerticalSpeed = "VerticalSpeed";
    public const string IsGrounded = "IsGrounded";
    public const string IsDashing = "IsDashing";
}
public class PlayerAnimatorController : MonoBehaviour
{
   Animator _animator;
   PlayerController _controller;
   

    private void Awake() 
   {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<PlayerController>();
   }
   
   private void Update() 
   {
        _animator.SetBool(CharacterAnimatorKeys.IsCrouching, _controller.IsCrouching); 

        if(InputHandler.instance.MoveInput.x != 0)
        {
        _animator.SetFloat(CharacterAnimatorKeys.HorizontalSpeed, InputHandler.instance.MoveInput.x / _controller.targetSpeed);
        }
        else
        {
            _animator.SetFloat(CharacterAnimatorKeys.HorizontalSpeed, 0f);
        }

        _animator.SetFloat(CharacterAnimatorKeys.VerticalSpeed, _controller._rb.velocity.y / _controller.force);

        if(!_controller.IsDashing && !_controller.IsJumping && !_controller.IsCrouching && !_controller.IsSliding)
        {
        _animator.SetBool(CharacterAnimatorKeys.IsGrounded, _controller.IsGrounded);
        }

        _animator.SetBool(CharacterAnimatorKeys.IsDashing, _controller.IsDashing);
    }
} 
