using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [SerializeField] private float rotationTime;

    public void Flip(float rotationY)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotationY + 180.0f, 0.0f));
        transform.DORotate(new Vector3(0.0f, rotationY, 0.0f), rotationTime, RotateMode.Fast);
    }
}
