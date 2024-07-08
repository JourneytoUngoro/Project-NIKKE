using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Scriptable Objects는 static한 data를 저장하는데 유용하다.
// ex) maxHealth -> O, currentHealth -> X

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Player Data")]
    public float maxHealth = 100.0f;
    public float maxPostureHealth = 100.0f;

    [Header("Move State")]
    public float moveSpeed = 10.0f;
    public float dashSpeed = 20.0f;
    public float dashInputTime = 1.0f;
    public float dashMaintainTime = 0.1f;

    [Header("Jump State")]
    public int maxJumpCount = 1;
    public float jumpSpeed = 5.0f;
    public float jumpInputBufferTime = 0.2f;

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMultiplier = 0.5f;
    public float gotoLandingStateTime = 2.0f;

    [Header("Dodge State")]
    public float dodgeSpeed = 20.0f;
    public float backstepSpeed = 20.0f;
    public float dodgeCoolDownTime = 1.0f;
    public float dodgeTime = 0.2f;
    public float dodgeDelay = 0.2f;

    [Header("Attack State")]
    public float meleeAttackDamage = 10.0f;
    public float meleeAttackRadius = 2.0f;
    public float rangedAttackDamage = 1.0f;
    public Vector2 rangedAttackDetectionSize;
    public float rangedAttackCoolDownTime = 0.1f;
    public int maxAmmo = 40;

    [Header("Wall Slide State")]
    public float wallSlideSpeed = 5.0f;

    [Header("Wall Jump State")]
    public Vector2 wallJumpAngleVector;
    public float wallJumpSpeed = 30.0f;
    public float wallJumpTime = 0.5f;
    public float preventInputXTime = 0.8f;

    [Header("Escape State")]
    public Vector2 escapeAngleVector;
    public float escapeSpeed = 100.0f;
    public float escapeTime = 0.1f;
    public float escapeCoolDownTime = 10.0f;
    public float escapeAttackDamage = 5.0f;

    [Header("Dash Attack State")]
    public float dashAttackTime = 0.5f;
    public float dashAttackCoolDowmTime = 2.0f;
    public float dashAttackSpeed = 20.0f;
    public float dashAttackDamage = 15.0f;
    public float dashAttackRadius = 1.5f;

    [Header("Crouch State")]
    public float crouchMoveSpeed;
    public Vector2 standColliderSize;
    public Vector2 crouchColliderSize;

    [Header("Defend State")]
    public float defendCoolDownTime = 1.0f;
    public float parryTime = 0.5f;

    [Header("Landing State")]
    public float landingTime = 1.0f;
}
