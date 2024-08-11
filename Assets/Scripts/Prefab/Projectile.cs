using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : AggressiveObject
{
    [SerializeField] private LayerMask whatIsTarget;

    private Rigidbody2D rigidBody;

    protected bool isTouchedGround;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isTouchedGround)
        {
            float angle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // Above function rotates projectile according to its direction
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer | whatIsTarget) != 0)
        {
            isTouchedGround = true;
            ReleaseObject();
            OnCollision();
        }
    }

    protected virtual void OnCollision()
    {
        
    }
}
