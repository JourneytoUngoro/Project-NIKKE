using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConnectorInteraction : TriggerInteractBase
{
    public enum Direction { None, Up, Down, Left, Right, All }

    [SerializeField] private Direction direction;
    [SerializeField] private SceneField targetScene;

    public override void Interact()
    {
        switch (direction)
        {
            case Direction.None:
                return;
            case Direction.Up:
                if (Manager.Instance.gameManager.player.rigidBody.velocity.y <= 0) return; break;
            case Direction.Down:
                if (Manager.Instance.gameManager.player.rigidBody.velocity.y >= 0) return; break;
            case Direction.Left:
                if (Manager.Instance.gameManager.player.rigidBody.velocity.x >= 0) return; break;
            case Direction.Right:
                if (Manager.Instance.gameManager.player.rigidBody.velocity.x <= 0) return; break;
            case Direction.All:
                break;
            default: return;
        }

        Manager.Instance.sceneTransitionManager.SceneTransition(targetScene, direction);
    }
}
