using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float disappearTime;
    private float initialAlpha;
    private float currentAlpha;
    private Coroutine coroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        currentAlpha = initialAlpha;
        coroutine = StartCoroutine(DecreaseAlpha());
    }

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    private void Update()
    {
        if (currentAlpha <= 0.001f)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator DecreaseAlpha()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(disappearTime / 10.0f);
        
        while (true)
        {
            yield return waitForSeconds;
            currentAlpha -= initialAlpha / 10.0f;
            spriteRenderer.color = new Color(1.0f, 0.5f, 1.0f, currentAlpha);
        }
    }

    public void SetAfterImage(SpriteRenderer afterImageCaller, float disappearTime, float initialAlpha)
    {
        this.disappearTime = disappearTime;
        this.initialAlpha = initialAlpha;
        spriteRenderer.sprite = afterImageCaller.sprite;
        transform.position = afterImageCaller.transform.position;
        transform.rotation = afterImageCaller.transform.rotation;
    }
}
