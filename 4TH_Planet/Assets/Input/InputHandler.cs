using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
	
	public static InputHandler instance;

	private GameControls controls;

	[Header("Input Values")]
	public Action<InputArgs> OnJumpPressed;
	public Action<InputArgs> OnJumpReleased;
	public Action<InputArgs> OnDash;
	public Action<InputArgs> OnCrouch;
	public Action<InputArgs> OnStandUp;
	public Action<InputArgs> OnAttack;
	

	public Vector2 MoveInput { get; private set; }


	private void Awake()
	{
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

		controls = new GameControls();

		#region Assign Inputs
		controls.Player.Walk.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
		controls.Player.Walk.canceled += ctx => MoveInput = Vector2.zero;

		controls.Player.Jump.performed += ctx => OnJumpPressed(new InputArgs { context = ctx });
		controls.Player.JumpUp.performed += ctx => OnJumpReleased(new InputArgs { context = ctx });
		controls.Player.Dash.performed += ctx => OnDash(new InputArgs { context = ctx });
		controls.Player.Crouch.performed += ctx => OnCrouch(new InputArgs { context = ctx });
		controls.Player.CrouchUp.performed += ctx => OnStandUp(new InputArgs { context = ctx });
		controls.Player.Attack.performed += ctx => OnAttack(new InputArgs { context = ctx });

		#endregion
	}

	#region Events
	public class InputArgs
	{
		public InputAction.CallbackContext context;
	}


	#endregion

	#region OnEnable/OnDisable
	private void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}
	#endregion
}

