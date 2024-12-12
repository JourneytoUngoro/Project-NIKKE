using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParryPrefab : PooledObject
{
    public Entity parentEntity { get; private set; }
    public OverlapCollider overlapCollider { get; private set; }
    
    private float parryStartTime;
    private float parryTime;
    private float parryDurationTime;
    private bool changeToShield;

    private bool isParried;

    private void OnEnable()
    {
        isParried = false;
        parryTime = 0.0f;
        parryDurationTime = 0.0f;
        changeToShield = true;
        parryStartTime = Time.time;
    }

    private void Update()
    {
        if (!isParried)
        {
            if (Time.time > parryStartTime + parryTime)
            {
                if (changeToShield)
                {
                    gameObject.layer = LayerMask.NameToLayer("ShieldLayer");
                }
                else
                {
                    if (gameObject.activeSelf)
                    {
                        ReleaseObject();
                    }
                }
            }
        }
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();

        Destroy(GetComponent<Collider2D>());
    }

    public void SetParryData(Entity parentEntity, float parryTime, float parryDurationTime, bool changeToShield, OverlapCollider overlapCollider)
    {
        this.parentEntity = parentEntity;
        this.parryTime = parryTime;
        this.parryDurationTime = parryDurationTime;
        this.changeToShield = changeToShield;
        this.overlapCollider = overlapCollider;

        if (overlapCollider.overlapBox)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.size = overlapCollider.boxSize;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, overlapCollider.boxRotation);
        }
        else if (overlapCollider.overlapCircle)
        {
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = overlapCollider.circleRadius;
        }
        transform.position = overlapCollider.centerTransform.position;
    }

    public void IsParried()
    {
        isParried = true;

        foreach (ShieldParryPrefab shieldParryPrefab in parentEntity.entityCombat.GetComponentsInChildren<ShieldParryPrefab>())
        {
            shieldParryPrefab.Invoke("ReleaseObject", parryDurationTime);
        }
    }
}
