using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    // move this to object pooling manager

    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private List<AfterImage> afterImages;
    [SerializeField] private float timeBetweenAfterImages;
    
    private bool alreadyStarted;
    private Coroutine activateAfterImage;

    public void StartAfterImage(SpriteRenderer afterImageCaller, float disappearTime, float initialAlpha)
    {
        if (alreadyStarted == false)
        {
            // activateAfterImage = StartCoroutine(ActivateAfterImage(timeBetweenAfterImages, afterImageCaller, disappearTime, initialAlpha));
            alreadyStarted = true;
        }
    }

    public void StopAfterImage()
    {
        alreadyStarted = false;
        if (activateAfterImage != null)
        {
            StopCoroutine(activateAfterImage);
        }
    }

    private AfterImage GetInactiveAfterImage(SpriteRenderer afterImageCaller, float disappearTime, float initialAlpha)
    {
        if (afterImages.Count > 0)
        {
            foreach (AfterImage afterImage in afterImages)
            {
                if (afterImage.gameObject.activeSelf == false)
                {
                    afterImage.SetAfterImage(afterImageCaller, disappearTime, initialAlpha);
                    return afterImage;
                }
            }
        }

        GameObject afterImagePrefab = Instantiate(this.afterImagePrefab);
        AfterImage afterImageComponent = afterImagePrefab.GetComponent<AfterImage>();
        afterImageComponent.transform.parent = transform;
        afterImageComponent.SetAfterImage(afterImageCaller, disappearTime, initialAlpha);
        afterImages.Add(afterImageComponent);
        return afterImageComponent;
    }

    private IEnumerator ActivateAfterImage(float timeBetweenAfterImages, SpriteRenderer afterImageCaller, float disappearTime, float initialAlpha)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(timeBetweenAfterImages);

        while (true)
        {
            AfterImage afterImage = GetInactiveAfterImage(afterImageCaller, disappearTime, initialAlpha);
            afterImage.gameObject.SetActive(true);
            yield return waitForSeconds;
        }
    }
}
