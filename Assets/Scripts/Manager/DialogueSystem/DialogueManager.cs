using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] Canvas dialogueCanvas;
    [SerializeField] GameObject dialoguePrefab;

    [field: SerializeField] public float totalDialogue { get; private set; }
    [field: SerializeField] public float dialogueTime { get; private set; }
    [field: SerializeField] public float dialogueMovementDelay { get; private set; }
    [field: SerializeField] public Vector2 initialOffset { get; private set; }
    [field: SerializeField] public float yDistanceBetweenDialogue { get; private set; }

    [HideInInspector] public List<Dialogue> usedDialogueQueue;
    [HideInInspector] public List<Dialogue> unusedDialogueQueue;
    [HideInInspector] public List<Dialogue> totalDialogueQueue;

    private List<string> reservedDialogueQueue;

    private void Awake()
    {
        usedDialogueQueue = new List<Dialogue>();
        unusedDialogueQueue = new List<Dialogue>();
        reservedDialogueQueue = new List<string>();

        if (unusedDialogueQueue.Count == 0)
        {
            for (int index = 0; index < totalDialogue; index++)
            {
                GameObject dialogueGameObject = Instantiate(dialoguePrefab);
                RectTransform rectTransform = dialogueGameObject.GetComponent<RectTransform>();
                Dialogue dialogue = dialogueGameObject.GetComponent<Dialogue>();
                rectTransform.anchoredPosition = new Vector2(-(rectTransform.sizeDelta.x + initialOffset.x), rectTransform.anchoredPosition.y);
                unusedDialogueQueue.Add(dialogue);
                totalDialogueQueue.Add(dialogue);
            }
        }

        foreach (Dialogue dialogue in totalDialogueQueue)
        {
            dialogue.rectTransform.SetParent(dialogueCanvas.transform);
            dialogue.rectTransform.localScale = Vector3.one;
        }
    }

    public void SendDialogue(string dialogue, Dialogue reservedDialogue = null)
    {
        if (reservedDialogue != null)
        {
            reservedDialogue.GetDialogue(dialogue);
            usedDialogueQueue.Add(reservedDialogue);
        }
        else
        {
            Dialogue currentDialogue;

            if (unusedDialogueQueue.Count == 0)
            {
                currentDialogue = usedDialogueQueue[0];
                currentDialogue.ForceReleaseDialogue(dialogue);
                reservedDialogueQueue.Add(dialogue);
            }
            else
            {
                currentDialogue = unusedDialogueQueue[0];

                unusedDialogueQueue.RemoveAt(0);
                currentDialogue.GetDialogue(dialogue);
                usedDialogueQueue.Add(currentDialogue);
            }
        }
    }

    public void ReorderDialogue()
    {
        for (int index = 0; index < totalDialogueQueue.Count; index++)
        {
            RectTransform rectTransform = totalDialogueQueue[index].rectTransform;

            if (rectTransform.IsVisibleFrom())
            {
                rectTransform.DOAnchorPosY(initialOffset.y - (index + 3) % 4 * yDistanceBetweenDialogue, dialogueMovementDelay);
            }
            else
            {
                rectTransform.DOAnchorPosY(initialOffset.y - (index + 3) % 4 * yDistanceBetweenDialogue, 0.0f);
            }
        }

        Dialogue topDialogue = totalDialogueQueue[0];
        totalDialogueQueue.RemoveAt(0);
        totalDialogueQueue.Add(topDialogue);
    }
}
