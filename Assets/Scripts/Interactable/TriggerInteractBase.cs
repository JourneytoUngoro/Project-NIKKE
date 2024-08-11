using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class TriggerInteractBase : MonoBehaviour, IInteractable
{
    public bool canInteract { get; set; }
    private enum InteractionType { Auto, Door, Object }

    [SerializeField] private InteractionType interactionType;

    private void Update()
    {
        if (canInteract)
        {
            switch (interactionType)
            {
                case InteractionType.Auto:
                    Interact();
                    break;

                case InteractionType.Door:
                    if (Manager.Instance.gameManager.player.inputHandler.doorInteractionInput)
                    {
                        Interact();
                    }
                    break;

                case InteractionType.Object:
                    if (Manager.Instance.gameManager.player.inputHandler.objectInteractionInput)
                    {
                        Interact();
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(Manager.Instance.gameManager.player.gameObject))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(Manager.Instance.gameManager.player.gameObject))
        {
            canInteract = false;
        }
    }

    public abstract void Interact();
}
