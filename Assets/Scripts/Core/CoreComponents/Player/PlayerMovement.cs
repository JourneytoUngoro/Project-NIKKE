using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [SerializeField] private CameraFollowObject cameraFollowObject;

    public void CheckIfShouldFlip(int inputX)
    {
        if (inputX != 0 && facingDirection != inputX)
        {
            Flip();
            if (facingDirection == 1)
            {
                cameraFollowObject.Flip(0.0f);
            }
            else
            {
                cameraFollowObject.Flip(180.0f);
            }
        }
    }
}
