using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldParry : PooledObject
{
    public OverlapCollider overlapCollider { get; private set; }
    public CombatAbility pertainedCombatAbility { get; private set; }
    
    private float parryStartTime;
    private float parryTime;
    private float parryDurationTime;
    private bool changeToShield = true;

    private bool isParried;

    private void OnEnable()
    {
        parryStartTime = Time.time;
    }

    private void OnDisable()
    {
        isParried = false;
        parryTime = 0.0f;
        parryDurationTime = 0.0f;
        changeToShield = true;
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
                    ReleaseObject();
                }
            }
        }
        else
        {
            if (Time.time > parryStartTime + parryDurationTime)
            {
                ReleaseObject();
            }
        }
    }

    public override void ReleaseObject()
    {
        base.ReleaseObject();

        Destroy(GetComponent<Collider2D>());
    }

    public void SetParryData(CombatAbility pertainedCombatAbility, float parryTime, float parryDurationTime, bool changeToShield, OverlapCollider overlapCollider)
    {
        this.pertainedCombatAbility = pertainedCombatAbility;
        this.parryTime = parryTime;
        this.parryDurationTime = parryDurationTime;
        this.changeToShield = changeToShield;
        this.overlapCollider = overlapCollider;
        gameObject.layer = LayerMask.NameToLayer("ParryLayer");

        if (overlapCollider.overlapBox)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = overlapCollider.boxSize;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, overlapCollider.boxRotation);
        }
        else if (overlapCollider.overlapCircle)
        {
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = overlapCollider.circleRadius;
        }
        transform.position = overlapCollider.centerTransform.position;
    }

    public void SetShieldData(CombatAbility pertainedCombatAbility, OverlapCollider overlapCollider)
    {
        this.pertainedCombatAbility = pertainedCombatAbility;
        this.overlapCollider = overlapCollider;
        gameObject.layer = LayerMask.NameToLayer("ShieldLayer");

        if (overlapCollider.overlapBox)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = overlapCollider.boxSize;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, overlapCollider.boxRotation);
        }
        else if (overlapCollider.overlapCircle)
        {
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = overlapCollider.circleRadius;
        }
        transform.position = overlapCollider.centerTransform.position;
    }

    public void IsParried()
    {
        isParried = true;
        parryStartTime = Time.time;

        foreach (ShieldParry shieldParryPrefab in pertainedCombatAbility.sourceEntity.entityCombat.GetComponentsInChildren<ShieldParry>())
        {
            shieldParryPrefab.Invoke("ReleaseObject", parryDurationTime);
        }
    }
}
