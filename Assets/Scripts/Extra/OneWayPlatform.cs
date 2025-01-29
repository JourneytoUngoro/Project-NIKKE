using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private bool onCollisionMutex;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCollisionMutex) return;
        onCollisionMutex = true;
        Debug.Log("OnCollisionEnter");
        Entity entity = collision.gameObject.GetComponent<Entity>();

        if (entity != null && entity.entityDetection.currentPlatform != platformCollider)
        {
            Physics2D.IgnoreCollision(platformCollider, collision.collider, true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onCollisionMutex = false;
        Debug.Log("OnCollisionExit");
        Entity entity = collision.gameObject.GetComponent<Entity>();

        if (entity != null && entity.entityDetection.currentPlatform != platformCollider)
        {
            Physics2D.IgnoreCollision(platformCollider, collision.collider, false);
        }
    }

    public void DisableCollision(Entity entity)
    {
        StartCoroutine(DisableCollisionCoroutine(entity));
    }

    private IEnumerator DisableCollisionCoroutine(Entity entity)
    {
        entity.entityDetection.groundedExceptions.Add(platformCollider);
        Physics2D.IgnoreCollision(platformCollider, entity.entityCollider);
        yield return new WaitUntil(() => { return entity.entityDetection.detectedPlatform != platformCollider; });
        yield return new WaitForSeconds(0.2f);
        entity.entityDetection.groundedExceptions.Remove(platformCollider);
        Physics2D.IgnoreCollision(platformCollider, entity.entityCollider, false);
    }
}
