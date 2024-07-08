using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BazierProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;

    private Rigidbody2D rigidBody;
    private Vector2 targetPosition;
    private Vector2 startPosition;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 prevPosition;
    private Vector2 currentPosition;

    private float distance;
    private float xDifference;
    private float transferTime;
    private float time;
    private float maxSpeed;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = FindObjectOfType<Player>().transform.position;

        distance = Vector2.Distance(transform.position, targetPosition);
        xDifference = targetPosition.x - transform.position.x;

        if (xDifference < 0)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
        }

        Random rand = new Random();
        float pointAXOffset = -(float)rand.NextDouble() * xDifference / 4.0f - xDifference / 4.0f;
        float pointAYOffset = (float)rand.NextDouble() * distance / 4.0f + distance / 4.0f;
        pointA = new Vector2(pointAXOffset + transform.position.x, pointAYOffset + Mathf.Max(transform.position.y, targetPosition.y));
        float pointBXOffset = (float)rand.NextDouble() * xDifference / 2.0f + xDifference / 4.0f;
        float pointBYOffset = ((float)rand.NextDouble() - 0.5f) * pointAYOffset + distance / 4.0f;
        pointB = new Vector2(pointBXOffset + pointA.x, pointBYOffset + pointA.y);

        time = 0.0f;
        transferTime = distance / projectileSpeed;
        currentPosition = transform.position;

        Destroy(gameObject, 3.0f);
    }

    private void FixedUpdate()
    {
        if (time < transferTime)
        {
            time += Time.fixedDeltaTime;
            prevPosition = currentPosition;
            currentPosition = BezierCurve(startPosition, pointA, pointB, targetPosition, time / transferTime);
            if (currentPosition.y < prevPosition.y)
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, Vector2.SignedAngle(Vector2.right, currentPosition - prevPosition));
                maxSpeed = Mathf.Max(maxSpeed, Vector2.Distance(prevPosition, currentPosition) / Time.fixedDeltaTime);
            }
            transform.position = currentPosition;
        }
        else
        {
            rigidBody.velocity = (currentPosition - prevPosition).normalized * maxSpeed;
        }
    }

    private Vector2 BezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float rate)
    {
        Vector2 a = Vector2.Lerp(p1, p2, rate);
        Vector2 b = Vector2.Lerp(p2, p3, rate);
        Vector2 c = Vector2.Lerp(p3, p4, rate);

        Vector2 d = Vector2.Lerp(a, b, rate);
        Vector2 e = Vector2.Lerp(b, c, rate);

        Vector2 f = Vector2.Lerp(d, e, rate);

        return f;
    }
}
