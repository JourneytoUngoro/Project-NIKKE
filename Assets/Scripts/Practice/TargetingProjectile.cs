using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TargetingProjectile : Projectile
{
    [SerializeField] private float minStep;
    [SerializeField] private float maxStep;
    [SerializeField] private float timeToMaxStep;

    private float currentStep;
    private float startTime;
    private Vector2 direction;
    private Transform targetEntity;

    protected override void Awake()
    {
        base.Awake();

        currentStep = minStep;
        startTime = Time.time;
        targetEntity = FindObjectOfType<Player>().transform;
    }

    private void FixedUpdate()
    {
        currentStep = Mathf.Lerp(minStep, maxStep, (Time.time - startTime) / timeToMaxStep);
        direction = (targetEntity.position - transform.position).normalized;

        Rotate(direction);
        rigidBody.velocity = transform.right * projectileSpeed;
    }

    private void Rotate(Vector2 direction)
    {
        Quaternion toRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, currentStep * Time.deltaTime);
    }
}
