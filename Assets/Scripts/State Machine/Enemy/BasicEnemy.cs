using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform detectionTransform;
    [SerializeField] private Vector2 detectionSize;
    [SerializeField] private float maxAgroRadius;

    private Rigidbody2D rigidBody;
    private EnemyCombat combat;

    private bool isPlayerDetected;
    private bool playerInAgroRange;
    private bool canAttack;

    private Timer attackCoolDownTimer = new Timer(1.0f);

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        combat = GetComponent<EnemyCombat>();
        canAttack = true;
        attackCoolDownTimer.timerAction += () => { canAttack = true; };
    }

    private void Update()
    {
        attackCoolDownTimer.Tick();

        if (isDetectingPlayer() && canAttack)
        {
            Debug.Log(gameObject.name + " Attack");
            combat.MeleeAttack();
            combat.RangedAttack();
            canAttack = false;
            attackCoolDownTimer.StartSingleUseTimer();
        }
    }

    private bool isDetectingPlayer()
    {
        return Physics2D.OverlapBox(detectionTransform.position, detectionSize, 0.0f, whatIsPlayer);
    }

    private bool isPlayerInAgroRange()
    {
        return Physics2D.OverlapCircle(transform.position, maxAgroRadius, whatIsPlayer);
    }
}