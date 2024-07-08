using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDataPersistance
{
    #region State Variables
    public PlayerStateMachine playerStateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerDodgeState dodgeState { get; private set; }
    public PlayerNormalAttackState normalAttackState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerWallGrabState wallGrabState { get; private set; }
    public PlayerWallAttackState wallAttackState { get; private set; }
    public PlayerLandingState landingState { get; private set; }
    public PlayerEscapeState escapeState { get; private set; }
    public PlayerDashAttackState dashAttackState { get; private set; }
    public PlayerCrouchIdleState crouchIdleState { get; private set; }
    public PlayerCrouchMoveState crouchMoveState { get; private set; }
    public PlayerReloadState reloadState { get; private set; }
    public PlayerBlockParryState blockParryState { get; private set; }
    public PlayerDashState dashState { get; private set; }

    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    #endregion

    #region Player Components
    public Animator animator { get; private set; }
    public Rigidbody2D rigidBody { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public CapsuleCollider2D playerCollider { get; private set; }
    public PlayerInput playerInput { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }
    public Core core { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerDetection detection { get; private set; }
    public PlayerCombat combat { get; private set; }
    public Stats stats { get; private set; }
    public StateMachineToAnimator stateMachineToAnimator { get; private set; }

    [field: SerializeField] public GameObject blockParryArea { get; private set; }
    [field: SerializeField] public Transform MeleeAttackTransform { get; private set; }
    [field: SerializeField] public Transform DashAttackTransform { get; private set; }
    [field: SerializeField] public Transform RangedAttackTransform { get; private set; }
    [field: SerializeField] public AfterImagePool afterImagePool { get; private set; }
    [field: SerializeField] public PhysicsMaterial2D defaultMaterial { get; private set; }
    [field: SerializeField] public PhysicsMaterial2D fullfrictionMaterial { get; private set; }
    #endregion

    #region Other Variables
    private Vector2 workSpace;
    public event Action OnInteractionInput;
    public GameData currentData;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerInput = GetComponent<PlayerInput>();
        inputHandler = GetComponent<PlayerInputHandler>();
        core = GetComponentInChildren<Core>();
        combat = GetComponent<PlayerCombat>();
        stateMachineToAnimator = GetComponent<StateMachineToAnimator>();
    }

    private void Start()
    {
        movement = core.GetCoreComponent<PlayerMovement>();
        detection = core.GetCoreComponent<PlayerDetection>();
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
        normalAttackState = new PlayerNormalAttackState(this, MeleeAttackTransform, "attack");
        dashAttackState = new PlayerDashAttackState(this, DashAttackTransform, "dashAttack");
        crouchIdleState = new PlayerCrouchIdleState(this, "crouch");
        crouchMoveState = new PlayerCrouchMoveState(this, "crouch");
        reloadState = new PlayerReloadState(this, "reload");
        blockParryState = new PlayerBlockParryState(this, "blockParry");
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
        Vector2 currentSize = playerCollider.size;
        Vector2 center = playerCollider.offset;

        playerCollider.direction = changeSize.x < changeSize.y ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;

        playerCollider.size = changeSize;
        center += (changeSize - currentSize) / 2.0f;
        playerCollider.offset = center;
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
