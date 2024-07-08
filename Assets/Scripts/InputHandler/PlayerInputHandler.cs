using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private Player player;

    private Timer jumpInputBufferTimer;

    public Vector2 movementInput { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public int actualInputX { get; private set; }
    public int fixedInputX { get; private set; }
    public bool preventInputX { get; private set; }
    public bool jumpInput { get; private set; }
    public bool jumpInputActive { get; private set; }
    public bool dodgeInput { get; private set; }
    public bool dodgeInputActive { get; private set; }
    public bool attackInput { get; private set; }
    public bool attackInputActive { get; private set; }
    public bool escapeInput { get; private set; }
    public bool dashAttackInput { get; private set; }
    public bool wideAttackInput { get; private set; }
    public bool ultimateInput { get; private set; }
    public bool blockParryInput { get; private set; }
    public bool blockParryInputActive { get; private set; }
    public bool interactionInput { get; private set; }
    public bool interactionInputActive { get; private set; }
    public bool menuInput { get; private set; }
    public bool menuInputActive { get; private set; }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();

        jumpInputBufferTimer = new Timer(player.PlayerData.jumpInputBufferTime);
        jumpInputBufferTimer.timerAction += InactiveJumpInput;
    }

    private void Update()
    {
        jumpInputBufferTimer.Tick();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        actualInputX = Mathf.RoundToInt(movementInput.x);
        normInputX = preventInputX ? fixedInputX : actualInputX;
        normInputY = Mathf.RoundToInt(movementInput.y);
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {  
        if (context.started)
        {
            jumpInput = true;
            jumpInputActive = true;
            jumpInputBufferTimer.StartSingleUseTimer();
        }
        
        if (context.canceled)
        {
            jumpInput = false;
        }
    }

    public void OnDodgeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dodgeInput = true;
            if (player.dodgeState.IsDodgeAvail())
            {
                dodgeInputActive = true;
            }
        }

        if (context.canceled)
        {
            dodgeInput = false;
        }
    }

    public void OnShieldParryInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            blockParryInput = true;
            blockParryInputActive = true;
        }

        if (context.canceled)
        {
            blockParryInput = false;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackInput = true;
            attackInputActive = true;
        }

        if (context.canceled)
        {
            attackInput = false;
        }
    }

    public void OnUpperSlashAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashAttackInput = true;
        }

        if (context.canceled)
        {
            dashAttackInput = false;
        }
    }

    public void OnWideAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            wideAttackInput = true;
        }

        if (context.canceled)
        {
            wideAttackInput = false;
        }
    }

    public void OnEscapeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            escapeInput = true;
        }

        if (context.canceled)
        {
            escapeInput = false;
        }
    }

    public void OnUltimateInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ultimateInput = true;
        }

        if (context.canceled)
        {
            ultimateInput = false;
        }
    }

    public void OnInteractionInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interactionInput = true;
            interactionInputActive = true;
        }

        if (context.canceled)
        {
            interactionInput = false;
        }
    }

    public void OnMenuInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            menuInput = true;
            menuInputActive = true;
        }

        if (context.canceled)
        {
            menuInput = false;
        }
    }

    public void InactiveJumpInput() => jumpInputActive = false;

    public void InactiveDodgeInput() => dodgeInputActive = false;

    public void InactiveInteractionInput() => interactionInputActive = false;

    public void InactiveAttackInput() => attackInputActive = false;

    public void InactiveMenuInput() => menuInputActive = false;

    public void PreventInputX(int fixedInputX)
    {
        if (preventInputX == false)
        {
            this.fixedInputX = fixedInputX;
            normInputX = fixedInputX;
            preventInputX = true;
        }
    }

    public void AvailInputX()
    {
        if (preventInputX)
        {
            preventInputX = false;
            normInputX = actualInputX;
        }
    }
}
