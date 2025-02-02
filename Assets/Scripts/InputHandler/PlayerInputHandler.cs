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
    private Timer rangedAttackInputBufferTimer;

    [HideInInspector] public Controls controls;

    public Vector2 movementInput { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public int actualInputX { get; private set; }
    public int fixedInputX { get; private set; }
    public bool preventInputX { get; private set; }
    public bool jumpInput { get; private set; } // 꾹 누르기
    public bool jumpInputActive { get; private set; } // input을 저장해야하거나, 버튼을 꾹 누를 경우 연속해서 사용되기를 원하지 않을때 그러나 누르는 즉시 실행될 수 있어야한다.
    public bool dodgeInputPressed { get; private set; }
    public bool attackInput { get; private set; }
    public bool attackInputActive { get; private set; }
    public bool rangedAttackInput { get; private set; }
    public bool rangedAttackInputActive { get; private set; }
    public bool rangedAttackInputPressed { get; private set; }
    public bool escapeInputPressed { get; private set; }
    public bool skillAttackInput { get; private set; }
    public bool skillAttackInputPressed { get; private set; }
    public bool ultimateInputPressed { get; private set; }
    public bool shieldParryInput { get; private set; }
    public bool shieldParryInputPressed { get; private set; }
    public bool objectInteractionInputPressed { get; private set; }
    public bool doorInteractionInputPressed { get; private set; }
    public bool menuInputPressed { get; private set; }
    public bool cureInputPressed { get; private set; }
    public bool mapInputPressed { get; private set; }

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
        rangedAttackInputBufferTimer = new Timer(0.1f);
        rangedAttackInputBufferTimer.timerAction += InactiveRangedAttackInput;
    }

    private void Update()
    {
        jumpInputBufferTimer.Tick();
        attackInputBufferTimer.Tick();
        
        cureInputPressed = controls.InGame.Cure.WasPressedThisFrame();
        objectInteractionInputPressed = controls.InGame.ObjectInteraction.WasPressedThisFrame();
        doorInteractionInputPressed = controls.InGame.DoorInteraction.WasPressedThisFrame();
        dodgeInputPressed = controls.InGame.Dodge.WasPressedThisFrame();
        skillAttackInputPressed = controls.InGame.SkillAttack.WasPressedThisFrame();
        escapeInputPressed = controls.InGame.Escape.WasPressedThisFrame();
        ultimateInputPressed = controls.InGame.Ultimate.WasPressedThisFrame();
        rangedAttackInputPressed = controls.InGame.RangedAttack.WasPressedThisFrame();
        shieldParryInputPressed = controls.InGame.ShieldParry.WasPressedThisFrame();
        menuInputPressed = controls.InGame.Menu.WasPressedThisFrame();
        mapInputPressed = controls.InGame.Map.WasPressedThisFrame();
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

    public void OnRangedAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            rangedAttackInput = true;
            rangedAttackInputActive = true;
            rangedAttackInputBufferTimer.StartSingleUseTimer();
        }

        if (context.canceled)
        {
            rangedAttackInput = false;
        }
    }

    public void OnSkillAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            skillAttackInput = true;
        }

        if (context.canceled)
        {
            skillAttackInput = false;
        }
    }

    public void InactiveJumpInput() => jumpInputActive = false;

    public void InactiveDodgeInput() => dodgeInputPressed = false;

    public void InactiveAttackInput() => attackInputActive = false;

    public void InactiveRangedAttackInput() => rangedAttackInputActive = false;

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
