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

    private Timer attackInputBufferTimer;

    [HideInInspector] public Controls controls;

    public Vector2 movementInput { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public int actualInputX { get; private set; }
    public int fixedInputX { get; private set; }
    public bool preventInputX { get; private set; }
    public bool jumpInput { get; private set; }
    public bool jumpInputActive { get; private set; } // input을 저장해야하거나, 버튼을 꾹 누를 경우 연속해서 사용되기를 원하지 않을때 그러나 누르는 즉시 실행될 수 있어야한다.
    public bool dodgeInput { get; private set; }
    public bool dodgeInputActive { get; private set; }
    public bool attackInput { get; private set; }
    public bool attackInputActive { get; private set; }
    public bool escapeInput { get; private set; }
    public bool dashAttackInput { get; private set; }
    public bool wideAttackInput { get; private set; }
    public bool ultimateInput { get; private set; }
    public bool shieldParryInput { get; private set; }
    public bool shieldParryInputActive { get; private set; }
    public bool objectInteractionInput { get; private set; }
    public bool doorInteractionInput { get; private set; }
    public bool menuInput { get; private set; }
    public bool cureInput { get; private set; }

    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<Player>();

        jumpInputBufferTimer = new Timer(player.playerData.jumpInputBufferTime);
        jumpInputBufferTimer.timerAction += InactiveJumpInput;
        attackInputBufferTimer = new Timer(0.1f);
        attackInputBufferTimer.timerAction += InactiveAttackInput;
    }

    private void Update()
    {
        jumpInputBufferTimer.Tick();
        attackInputBufferTimer.Tick();
        
        cureInput = controls.InGame.Cure.WasPressedThisFrame();
        objectInteractionInput = controls.InGame.ObjectInteraction.WasPressedThisFrame();
        doorInteractionInput = controls.InGame.DoorInteraction.WasPressedThisFrame();
        dodgeInputActive = controls.InGame.Dodge.WasPressedThisFrame();
        shieldParryInputActive = controls.InGame.ShieldParry.WasPressedThisFrame();
        menuInput = controls.InGame.Menu.WasPressedThisFrame();
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
            shieldParryInput = true;
        }

        if (context.canceled)
        {
            shieldParryInput = false;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            attackInput = true;
            attackInputActive = true;
            attackInputBufferTimer.StartSingleUseTimer();
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

    public void InactiveJumpInput() => jumpInputActive = false;

    public void InactiveDodgeInput() => dodgeInputActive = false;

    public void InactiveAttackInput() => attackInputActive = false;

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
