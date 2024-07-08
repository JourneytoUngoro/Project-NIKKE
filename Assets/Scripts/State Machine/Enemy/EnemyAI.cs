using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : CoreComponent
{
    [Header("Path Finding")]
    [SerializeField] private Transform target;
    [SerializeField] private float nextWaypointDistance = 3.0f;
    [SerializeField] private float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    [SerializeField] private float speed = 200.0f;
    [SerializeField] private float jumpHeightRequirement;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    private Seeker seeker;
    private Rigidbody2D rigidBody;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rigidBody = transform.GetComponentInParent<Rigidbody2D>();
    }

    private void Start()
    {
        InvokeRepeating("UpdatePath", 0.0f, 0.5f);
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rigidBody.position, target.position, OnPathComplete);
        }
    }

    private void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath= false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rigidBody.position).normalized;

        Vector2 force = direction * speed * Time.deltaTime;

        rigidBody.AddForce(force);

        float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
