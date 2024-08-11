using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }
    private TMP_Text textMeshPro;
    private Coroutine releaseDialogueCoroutine;
    private Coroutine forceReleaseDialogueCoroutine;

    private void Awake()
    {
        textMeshPro = GetComponentInChildren<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void GetDialogue(string text)
    {
        textMeshPro.text = text;
        rectTransform.anchoredPosition = new Vector2(-(rectTransform.sizeDelta.x + Manager.Instance.dialogueManager.initialOffset.x), Manager.Instance.dialogueManager.initialOffset.y - Manager.Instance.dialogueManager.yDistanceBetweenDialogue * Manager.Instance.dialogueManager.usedDialogueQueue.Count);
        rectTransform.DOAnchorPosX(Manager.Instance.dialogueManager.initialOffset.x, Manager.Instance.dialogueManager.dialogueMovementDelay);
        if (forceReleaseDialogueCoroutine != null)
        {
            StopCoroutine(forceReleaseDialogueCoroutine);
        }
        releaseDialogueCoroutine = StartCoroutine(ReleaseDialogueCoroutine());
    }

    public void ForceReleaseDialogue(string dialogue)
    {
        if (releaseDialogueCoroutine != null)
        {
            StopCoroutine(releaseDialogueCoroutine);
        }
        forceReleaseDialogueCoroutine = StartCoroutine(ForceReleaseDialogueCoroutine(dialogue));
    }

    public void MoveUpward()
    {
        rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + Manager.Instance.dialogueManager.yDistanceBetweenDialogue, Manager.Instance.dialogueManager.dialogueMovementDelay);
    }

    private IEnumerator ReleaseDialogueCoroutine()
    {
        yield return new WaitForSeconds(Manager.Instance.dialogueManager.dialogueTime + Manager.Instance.dialogueManager.dialogueMovementDelay);
        rectTransform.DOAnchorPosX(-(rectTransform.sizeDelta.x + Manager.Instance.dialogueManager.initialOffset.x), Manager.Instance.dialogueManager.dialogueMovementDelay);
        yield return new WaitForSeconds(Manager.Instance.dialogueManager.dialogueMovementDelay);
        Manager.Instance.dialogueManager.usedDialogueQueue.Remove(this);
        Manager.Instance.dialogueManager.unusedDialogueQueue.Add(this);
        Manager.Instance.dialogueManager.ReorderDialogue();
    }

    private IEnumerator ForceReleaseDialogueCoroutine(string dialogue)
    {
        rectTransform.DOAnchorPosX(-(rectTransform.sizeDelta.x + Manager.Instance.dialogueManager.initialOffset.x), Manager.Instance.dialogueManager.dialogueMovementDelay);
        yield return new WaitForSeconds(Manager.Instance.dialogueManager.dialogueMovementDelay);
        Manager.Instance.dialogueManager.ReorderDialogue();
        yield return new WaitForSeconds(Manager.Instance.dialogueManager.dialogueMovementDelay);
        Manager.Instance.dialogueManager.usedDialogueQueue.Remove(this);
        Manager.Instance.dialogueManager.SendDialogue(dialogue, this);
    }
}
