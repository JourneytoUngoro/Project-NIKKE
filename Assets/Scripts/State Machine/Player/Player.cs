using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    #region State Variables
    public PlayerStateMachine playerStateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }
    public PlayerMeleeAttackState meleeAttackState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerWallGrabState wallGrabState { get; private set; }
    public PlayerWallAttackState wallAttackState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerEscapeState escapeState { get; private set; }
    public PlayerJumpAttackState dashAttackState { get; private set; }
    public PlayerCrouchIdleState crouchIdleState { get; private set; }
    public PlayerCrouchMoveState crouchMoveState { get; private set; }
    public PlayerReloadState reloadState { get; private set; }
    public PlayerShieldParryState shieldParryState { get; private set; }
    public PlayerKnockbackState knockbackState { get; private set; }
    public PlayerStunnedState stunnedState { get; private set; }
    public PlayerCureState cureState { get; private set; }

    [field: SerializeField] public PlayerData playerData { get; private set; }
    #endregion

    #region Player Components
    public PlayerMovement movement { get; protected set; }
    public PlayerDetection detection { get; protected set; }
    public PlayerCombat combat { get; protected set; }
    public PlayerStats stats { get; protected set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    #endregion

    #region Other Variables
    
    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerInput = GetComponent<PlayerInput>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    protected override void Start()
    {
        base.Start();

        /*movement = core.GetCoreComponent<PlayerMovement>();
        detection = core.GetCoreComponent<PlayerDetection>();
        combat = core.GetCoreComponent<PlayerCombat>();
        stats = core.GetCoreComponent<PlayerStats>();*/
        movement = entityMovement as PlayerMovement;
        detection = entityDetection as PlayerDetection;
        combat = entityCombat as PlayerCombat;
        stats = entityStats as PlayerStats;

        playerStateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, "idle");
        moveState = new PlayerMoveState(this, "move");
        jumpState = new PlayerJumpState(this, "inAir");
        inAirState = new PlayerInAirState(this, "inAir");
        dodgeState = new PlayerDodgeState(this, "dodge");
        wallSlideState = new PlayerWallSlideState(this, "wallSlide");
        wallGrabState = new PlayerWallGrabState(this, "wallGrab");
        wallJumpState = new PlayerWallJumpState(this, "inAir");
        landingState = new PlayerLandingState(this, "crouch");
        escapeState = new PlayerEscapeState(this, "escape");
        meleeAttackState = new PlayerMeleeAttackState(this, "meleeAttack");
        dashAttackState = new PlayerJumpAttackState(this, "jumpAttack");
        crouchIdleState = new PlayerCrouchIdleState(this, "crouch");
        crouchMoveState = new PlayerCrouchMoveState(this, "crouch");
        reloadState = new PlayerReloadState(this, "reload");
        shieldParryState = new PlayerShieldParryState(this, "blockParry");
        knockbackState = new PlayerKnockbackState(this, "knockback");
        cureState = new PlayerCureState(this, "cure");

        playerStateMachine.Initialize(idleState);
    }

    private void Update()
    {
        if (inputHandler.menuInput)
        {
            if (Manager.Instance.uiManager.savePointMenu.activeSelf)
            {
                Manager.Instance.uiManager.CloseSavePointMenu();
            }
            else
            {
                if (Manager.Instance.uiManager.pauseMenu.activeSelf)
                {
                    Manager.Instance.uiManager.ClosePauseMenu();
                }
                else
                {
                    Manager.Instance.uiManager.OpenPauseMenu();
                }
            }
        }

        playerStateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        playerStateMachine.currentState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SavePoint"))
        {
            Manager.Instance.uiManager.OpenNotificationWindow("Press Z to interact");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SavePoint"))
        {
            Manager.Instance.uiManager.CloseNotificationWindow();
        }
    }

    public void SetCapsuleColliderSize(Vector2 changeSize)
    {
        CapsuleCollider2D entityCollider = this.entityCollider as CapsuleCollider2D;
        Vector2 currentSize = entityCollider.size;
        Vector2 center = entityCollider.offset;

        entityCollider.direction = changeSize.x < changeSize.y ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;

        entityCollider.size = changeSize;
        center += (changeSize - currentSize) / 2.0f;
        entityCollider.offset = center;
    }

    public void LoadData(GameData data)
    {
        // player's position should be set only when loading from MainMenu
        if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
        {
            transform.position = data.lastSavePosition;
        }
    }

    public void SaveData(GameData data)
    {
        data.lastSavePosition = transform.position;
    }
}
