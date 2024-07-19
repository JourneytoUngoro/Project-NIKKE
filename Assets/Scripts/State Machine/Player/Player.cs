using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Entity, IDataPersistance
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
    public PlayerBlockParryState shieldParryState { get; private set; }
    public PlayerDashState dashState { get; private set; }

    [field: SerializeField] public PlayerData playerData { get; private set; }
    #endregion

    #region Player Components
    public PlayerMovement movement { get; protected set; }
    public PlayerDetection detection { get; protected set; }
    public PlayerCombat combat { get; protected set; }
    public Stats stats { get; protected set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }

    [field: SerializeField] public GameObject blockParryArea { get; private set; }
    #endregion

    #region Other Variables
    public event Action OnInteractionInput;
    public GameData currentData;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerInput = GetComponent<PlayerInput>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        movement = core.GetCoreComponent<PlayerMovement>();
        detection = core.GetCoreComponent<PlayerDetection>();
        combat = core.GetCoreComponent<PlayerCombat>();
        stats = core.GetCoreComponent<Stats>();

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
        shieldParryState = new PlayerBlockParryState(this, "blockParry");
        dashState = new PlayerDashState(this, "move");

        playerStateMachine.Initialize(idleState);
    }

    private void Update()
    {
        if (inputHandler.interactionInputActive)
        {
            inputHandler.InactiveInteractionInput();
            OnInteractionInput?.Invoke();
        }
        else if (inputHandler.menuInputActive)
        {
            inputHandler.InactiveMenuInput();
            if (Manager.instance.uiManager.savePointMenu.activeSelf)
            {
                Manager.instance.uiManager.CloseSavePointMenu();
            }
            else
            {
                if (Manager.instance.uiManager.pauseMenu.activeSelf)
                {
                    Manager.instance.uiManager.ClosePauseMenu();
                }
                else
                {
                    Manager.instance.uiManager.OpenPauseMenu();
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
            Manager.instance.uiManager.OpenNotificationWindow("Press Z to interact");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SavePoint"))
        {
            Manager.instance.uiManager.CloseNotificationWindow();
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
        this.currentData = data;
    }

    public void SaveData(GameData data)
    {
        data = this.currentData;
    }
}
