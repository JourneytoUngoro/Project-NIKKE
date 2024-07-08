using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TargetingProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCollider;
    [SerializeField] private float minStep;
    [SerializeField] private float maxStep;
    [SerializeField] private float timeToMaxStep;
    [SerializeField] private float projectileSpeed;

    private Rigidbody2D rb;
    private float step;
    private float startTime;
    private Vector2 direction;
    private Transform currentTarget;

    private bool isTouchedGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        step = minStep;
        startTime = Time.time;
        currentTarget = FindObjectOfType<Player>().transform;
        // Destroy(gameObject, 10.0f);
    }

    private void Update()
    {
        /*if (!isTouchedGround)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // 위 함수는 투사체가 진행 하는 방향에 따라서 회전시켜준다.
        }*/
    }

    private void FixedUpdate()
    {
        step = Mathf.Lerp(minStep, maxStep, (Time.time - startTime) / timeToMaxStep);
        direction = (currentTarget.position - transform.position).normalized;

        Rotate(direction);
        rb.velocity = transform.right * projectileSpeed;
        // rb.velocity = direction * projectileSpeed;
    }

    private void Rotate(Vector2 direction)
    {
        Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, step * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isTouchedGround = true;
            Destroy(gameObject);
        }
    }
}
