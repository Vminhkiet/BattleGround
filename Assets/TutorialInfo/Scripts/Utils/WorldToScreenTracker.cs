using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldToScreenTracker : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        if(mainCameraTransform == null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
            this.enabled = false;
    }

    private void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            Vector3 directionToCamera = mainCameraTransform.position - transform.position;

            directionToCamera.y = 0;

            if (directionToCamera == Vector3.zero)
            {
                return;
            }

            Quaternion targetRotationWorld = Quaternion.LookRotation(directionToCamera);

            transform.localRotation = Quaternion.Euler(0f, targetRotationWorld.eulerAngles.y, 0f);
        }
    }
}
