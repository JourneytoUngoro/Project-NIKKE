using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : PooledObject
{
    [SerializeField] private float initialAlpha;
    [SerializeField] private float disappearTime;
    
    public SpriteRenderer spriteRenderer { get; private set; }

    private float elapsedTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        elapsedTime = 0.0f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        Color nextColor = spriteRenderer.color;

        nextColor.a = Mathf.Clamp(DOVirtual.EasedValue(initialAlpha, 0.0f, elapsedTime / disappearTime, Ease.Linear), 0.0f, initialAlpha);

        spriteRenderer.color = nextColor;

        if (elapsedTime > disappearTime)
        {
            ReleaseObject();
        }
    }
}
