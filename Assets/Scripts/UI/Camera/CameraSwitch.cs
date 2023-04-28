using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera cam;
    public static CinemachineVirtualCamera activeCamera = null;

    void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        activeCamera = camera;

        foreach (CinemachineVirtualCamera vcam in CameraManager.Instance.cameras)
        {
            if (vcam != camera && vcam.Priority != 0)
            {
                vcam.Priority = 0;
                vcam.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && activeCamera != cam)
        {
            cam.gameObject.SetActive(true);
            SwitchCamera(cam);
        }
    }
}
